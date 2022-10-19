using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace NEA
{
    class TextBlockCombination
    {
        private TextBlock textBlock;

        private string previousValue;
        
        public TextBlockCombination(TextBlock textBlock, string previousValue)
        {
            this.textBlock = textBlock;
            this.previousValue = previousValue;
        }

        public string getPreviousValue()
        {
            return this.previousValue;
        }

        public TextBlock getTextBlock()
        {
            return this.textBlock;
        }

    }
}
