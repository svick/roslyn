﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Composition
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.DiagnosticComments.CodeFixes
    <ExportCodeFixProvider(LanguageNames.VisualBasic, Name:=PredefinedCodeFixProviderNames.RemoveDocCommentNode), [Shared]>
    Friend Class VisualBasicRemoveDocCommentNodeCodeFixProvider
        Inherits AbstractRemoveDocCommentNodeCodeFixProvider(Of XmlElementSyntax)

        ''' <summary>
        ''' XML comment tag with identical attributes
        ''' </summary>
        Private Const s_BC42305 As String = NameOf(s_BC42305)

        ''' <summary>
        ''' XML comment tag is not permitted on a 'sub' language element
        ''' </summary>
        Private Const s_BC42306 As String = NameOf(s_BC42306)

        ''' <summary>
        ''' XML comment type parameter does not match a type parameter
        ''' </summary>
        Private Const s_BC42307 As String = NameOf(s_BC42307)

        ''' <summary>
        ''' XML comment tag 'returns' is not permitted on a 'WriteOnly' property
        ''' </summary>
        Private Const s_BC42313 As String = NameOf(s_BC42313)

        ''' <summary>
        ''' XML comment tag 'returns' is not permitted on a 'declare sub' language element
        ''' </summary>
        Private Const s_BC42315 As String = NameOf(s_BC42315)

        ''' <summary>
        ''' XML comment type parameter does not match a type parameter
        ''' </summary>
        Private Const s_BC42317 As String = NameOf(s_BC42317)

        Friend ReadOnly Id As ImmutableArray(Of String) = ImmutableArray.Create(s_BC42305, s_BC42306, s_BC42307, s_BC42313, s_BC42315, s_BC42317)

        Public Overrides ReadOnly Property FixableDiagnosticIds As ImmutableArray(Of String)
            Get
                Return Id
            End Get
        End Property

        Protected Overrides ReadOnly Property DocCommentSignifierToken As String
            Get
                Return "'''"
            End Get
        End Property

        Protected Overrides Function GetRevisedDocCommentTrivia(docCommentText As String) As SyntaxTriviaList
            Return SyntaxFactory.ParseLeadingTrivia(docCommentText)
        End Function
    End Class
End Namespace