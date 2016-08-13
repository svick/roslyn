﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Microsoft.CodeAnalysis.CodeGen
{
    internal sealed class CompilationTestData
    {
        internal struct MethodData
        {
            public readonly ILBuilder ILBuilder;
            public readonly IMethodSymbol Method;

            public MethodData(ILBuilder ilBuilder, IMethodSymbol method)
            {
                this.ILBuilder = ilBuilder;
                this.Method = method;
            }
        }

        // The map is used for storing a list of methods and their associated IL.
        public readonly ConcurrentDictionary<IMethodSymbol, MethodData> Methods = new ConcurrentDictionary<IMethodSymbol, MethodData>();

        // The emitted module.
        public CommonPEModuleBuilder Module;

        public Func<object> SymWriterFactory;

        public ILBuilder GetIL(Func<IMethodSymbol, bool> predicate)
        {
            return Methods.Single(p => predicate(p.Key)).Value.ILBuilder;
        }

        private ImmutableDictionary<string, MethodData> _lazyMethodsByName;

        // Returns map indexed by name for those methods that have a unique name.
        public ImmutableDictionary<string, MethodData> GetMethodsByName()
        {
            if (_lazyMethodsByName == null)
            {
                var map = new Dictionary<string, MethodData>();
                foreach (var pair in Methods)
                {
                    var name = GetMethodName(pair.Key);
                    if (map.ContainsKey(name))
                    {
                        map[name] = default(MethodData);
                    }
                    else
                    {
                        map.Add(name, pair.Value);
                    }
                }
                var methodsByName = map.Where(p => p.Value.Method != null).ToImmutableDictionary();
                Interlocked.CompareExchange(ref _lazyMethodsByName, methodsByName, null);
            }
            return _lazyMethodsByName;
        }

        private static readonly SymbolDisplayFormat s_testDataKeyFormat = new SymbolDisplayFormat(
            compilerInternalOptions: SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames,
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
            memberOptions:
                SymbolDisplayMemberOptions.IncludeParameters |
                SymbolDisplayMemberOptions.IncludeContainingType |
                SymbolDisplayMemberOptions.IncludeExplicitInterface,
            parameterOptions:
                SymbolDisplayParameterOptions.IncludeParamsRefOut |
                SymbolDisplayParameterOptions.IncludeExtensionThis |
                SymbolDisplayParameterOptions.IncludeType,
            // Not showing the name is important because we visit parameters to display their
            // types.  If we visited their types directly, we wouldn't get ref/out/params.
            miscellaneousOptions:
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays |
                SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName);

        private static readonly SymbolDisplayFormat s_testDataOperatorKeyFormat = new SymbolDisplayFormat(
             s_testDataKeyFormat.CompilerInternalOptions,
             s_testDataKeyFormat.GlobalNamespaceStyle,
             s_testDataKeyFormat.TypeQualificationStyle,
             s_testDataKeyFormat.GenericsOptions,
             s_testDataKeyFormat.MemberOptions | SymbolDisplayMemberOptions.IncludeType,
             s_testDataKeyFormat.ParameterOptions,
             s_testDataKeyFormat.DelegateStyle,
             s_testDataKeyFormat.ExtensionMethodStyle,
             s_testDataKeyFormat.PropertyStyle,
             s_testDataKeyFormat.LocalOptions,
             s_testDataKeyFormat.KindOptions,
             s_testDataKeyFormat.MiscellaneousOptions);

        private static string GetMethodName(IMethodSymbol methodSymbol)
        {
            var format = (methodSymbol.MethodKind == MethodKind.UserDefinedOperator) ?
                s_testDataOperatorKeyFormat :
                s_testDataKeyFormat;
            return methodSymbol.ToDisplayString(format);
        }
    }
}
