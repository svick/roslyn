﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Structure;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Structure;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Structure.MetadataAsSource
{
    public class EventDeclarationStructureTests : AbstractCSharpSyntaxNodeStructureTests<EventDeclarationSyntax>
    {
        protected override string WorkspaceKind => CodeAnalysis.WorkspaceKind.MetadataAsSource;
        internal override AbstractSyntaxStructureProvider CreateProvider() => new EventDeclarationStructureProvider();

        [Fact, Trait(Traits.Feature, Traits.Features.MetadataAsSource)]
        public async Task NoCommentsOrAttributes()
        {
            const string code = @"
class Goo
{
    {|hint:public event EventArgs $$goo {|textspan:{ add; remove; }|}|}
}";

            await VerifyBlockSpansAsync(code,
                Region("textspan", "hint", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.MetadataAsSource)]
        public async Task WithAttributes()
        {
            const string code = @"
class Goo
{
    {|hint1:{|textspan1:[Goo]
    |}{|hint2:public event EventArgs $$goo {|textspan2:{ add; remove; }|}|}|}
}";

            await VerifyBlockSpansAsync(code,
                Region("textspan1", "hint1", CSharpStructureHelpers.Ellipsis, autoCollapse: true),
                Region("textspan2", "hint2", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.MetadataAsSource)]
        public async Task WithCommentsAndAttributes()
        {
            const string code = @"
class Goo
{
    {|hint1:{|textspan1:// Summary:
    //     This is a summary.
    [Goo]
    |}{|hint2:event EventArgs $$goo {|textspan2:{ add; remove; }|}|}|}
}";

            await VerifyBlockSpansAsync(code,
                Region("textspan1", "hint1", CSharpStructureHelpers.Ellipsis, autoCollapse: true),
                Region("textspan2", "hint2", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.MetadataAsSource)]
        public async Task WithCommentsAttributesAndModifiers()
        {
            const string code = @"
class Goo
{
    {|hint1:{|textspan1:// Summary:
    //     This is a summary.
    [Goo]
    |}{|hint2:public event EventArgs $$goo {|textspan2:{ add; remove; }|}|}|}
}";

            await VerifyBlockSpansAsync(code,
                Region("textspan1", "hint1", CSharpStructureHelpers.Ellipsis, autoCollapse: true),
                Region("textspan2", "hint2", CSharpStructureHelpers.Ellipsis, autoCollapse: true));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Outlining)]
        public async Task TestEvent3()
        {
            const string code = @"
class C
{
    $$event EventHandler E
    {
        add { }
        remove { }
    }

    event EventHandler E2
    {
        add { }
        remove { }
    }
}";

            await VerifyBlockSpansAsync(code,
                new BlockSpan(
                    isCollapsible: true,
                    textSpan: TextSpan.FromBounds(38, 91),
                    hintSpan: TextSpan.FromBounds(18, 89),
                    type: BlockTypes.Nonstructural,
                    bannerText: CSharpStructureHelpers.Ellipsis,
                    autoCollapse: true));
        }
    }
}
