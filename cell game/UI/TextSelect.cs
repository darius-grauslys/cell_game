using isometricgame.GameEngine.Systems.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.UI
{
    public struct TextOption
    {
        public string option;
        public readonly Action optionAction;

        public TextOption(string option, Action optionAction)
        {
            this.option = option;
            this.optionAction = optionAction;
        }

        public override string ToString()
        {
            return option;
        }

        public static implicit operator string(TextOption option) => option.option;
    }

    public class TextSelect
    {
        private int index;
        public int Index => index;
        private TextOption[] options;
        public TextOption[] Options => options;

        public TextSelect(TextOption[] options)
        {
            this.options = options;
        }

        public void Draw(RenderService renderService, TextDisplayer textDisplayer, int x, int y)
        {
            string text;
            for (int i = 0; i < options.Length; i++)
            {
                text = (index == i) ? String.Format("<{0}>", options[i]) : options[i];
                textDisplayer.DrawText(renderService, text, "font", -18 * text.Length / 2 + x, -30 * i + y);
            }
        }

        public void SelectOption()
        {
            options[index].optionAction();
        }

        public void OffsetIndex(int offset)
        {
            offset = (offset + options.Length) % options.Length;
            index = (offset + index) % options.Length;
        }

        public void SetIndex(int index)
        {
            if (index < 0) index = -index;
            index = index % options.Length;
        }
    }
}
