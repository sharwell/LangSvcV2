﻿namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.ComponentModel.Composition;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;
    using Tvl.VisualStudio.Text.Commenter;
    using Tvl.VisualStudio.Text.Commenter.Interfaces;

    [Export(typeof(ICommenterProvider))]
    [ContentType(PhpConstants.PhpContentType)]
    public sealed class PhpCommenterProvider : ICommenterProvider
    {
        private static readonly LineCommentFormat LineCommentFormat = new LineCommentFormat("//");
        private static readonly BlockCommentFormat BlockCommentFormat = new BlockCommentFormat("/*", "*/");

        public ICommenter TryCreateCommenter([NotNull] ITextBuffer textBuffer)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));

            Func<FormatCommenter> factory = () => new FormatCommenter(textBuffer, LineCommentFormat, BlockCommentFormat);
            return textBuffer.Properties.GetOrCreateSingletonProperty<FormatCommenter>(factory);
        }
    }
}
