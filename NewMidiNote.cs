using System;
using System.Collections.Generic;
using System.Text;

namespace NEA
{
    class NewMidiNote
    {
        public int position { private set; get; }
        public string note { private set; get; }
        public int octave { private set; get; }


        public NewMidiNote(int position, string note, int octave)
        {
            this.position = position;
            this.note = note;
            this.octave = octave;
        }
    }
}
