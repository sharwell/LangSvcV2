﻿namespace Tvl.VisualStudio.Text.Tagging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Language.Parsing;

    internal class BackgroundParserErrorTagger : ITagger<SquiggleTag>
    {
        private ITagSpan<SquiggleTag>[] _tags;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public BackgroundParserErrorTagger(ITextBuffer textBuffer, IBackgroundParser backgroundParser)
        {
            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;

            this.BackgroundParser.ParseComplete += new EventHandler<ParseResultEventArgs>(BackgroundParserParseComplete);
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public IBackgroundParser BackgroundParser
        {
            get;
            private set;
        }

        public IEnumerable<ITagSpan<SquiggleTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _tags;
        }

        private void BackgroundParserParseComplete(object sender, ParseResultEventArgs e)
        {
            var startToken = (IToken)((AntlrParseResultEventArgs)e).Result.Start;
            SnapshotCharStream input = (SnapshotCharStream)startToken.InputStream;
            _tags = e.Errors.Select(error => new TagSpan<SquiggleTag>(new SnapshotSpan(input.Snapshot, error.Span), new SquiggleTag(StandardErrorTypeService.SyntaxError, error.Message))).ToArray();

            var snapshot = TextBuffer.CurrentSnapshot;
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }

        private void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
