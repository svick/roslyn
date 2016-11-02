﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.ConvertToInterpolatedString;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.ConvertToInterpolatedString
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = PredefinedCodeRefactoringProviderNames.ConvertToInterpolatedString + "2"), Shared]
    internal class CSharpConvertConcatenationToInterpolatedStringRefactoringProvider :
        AbstractConvertConcatenationToInterpolatedStringRefactoringProvider
    {
        protected override SyntaxNode CreateInterpolatedString(
            SyntaxToken firstStringToken,
            List<SyntaxNode> pieces)
        {
            //InterpolatedStringContentSyntax
            //return SyntaxFactory.InterpolatedStringExpression()

            var isVerbatim = firstStringToken.IsVerbatimStringLiteral();
            var firstTokenText = isVerbatim ? "$@\"" : "$\"";

            var startToken = SyntaxFactory.Token(
                pieces.First().GetLeadingTrivia(),
                SyntaxKind.InterpolatedStringStartToken,
                firstTokenText,
                "",
                SyntaxFactory.TriviaList());

            var endToken = SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.InterpolatedStringEndToken,
                pieces.Last().GetTrailingTrivia());

            var content = new List<InterpolatedStringContentSyntax>();
            foreach (var piece in pieces)
            {
                if (piece.Kind() == SyntaxKind.StringLiteralExpression)
                {
                    var text = piece.GetFirstToken().Text;
                    var textWithoutQuotes = isVerbatim
                        ? text.Substring("@'".Length, text.Length - "@''".Length)
                        : text.Substring("'".Length, text.Length - "''".Length);
                    content.Add(SyntaxFactory.InterpolatedStringText(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(),
                            SyntaxKind.InterpolatedStringTextToken,
                            textWithoutQuotes,
                            "",
                            SyntaxFactory.TriviaList())));
                }
                else
                {
                    content.Add(SyntaxFactory.Interpolation((ExpressionSyntax)piece.WithoutTrivia()));
                }
            }

            var expression = SyntaxFactory.InterpolatedStringExpression(
                startToken, SyntaxFactory.List(content), endToken);

            return expression;
        }
    }
}