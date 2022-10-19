using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static NEA.ExtensionMethods;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System.Drawing;
using Microsoft.Win32;

namespace NEA
{
    public partial class MainWindow : Window
    {
     /*
        Fields: 37-55
        General Methods: 55-68
        Section C Methods: 68-177
        Section D Methods: 177-377
        Section E Methods: 377-460  
     */
     //Fields 
        
        private List<string> Keys = new List<string>() { "A_ma", "A_mi", "A8ma", "A8mi", "B_ma", "B_mi", "C_ma", "C_mi", "C8ma", "C8mi", "D_ma", "D_mi", "D8ma", "D8mi", "E_ma", "E_mi", "F_ma", "F_mi", "F8ma", "F8mi", "G_ma", "G_mi", "G8ma", "G8mi" };
        private List<string> NotesScientific = new List<string>() { "C_", "C8", "D_", "D8", "E_", "F_", "F8", "G_", "G8", "A_", "A8", "B_"};
        private List<string> Notes = new List<string>() { "A_", "A8", "B_","C_", "C8", "D_", "D8", "E_", "F_", "F8", "G_", "G8" };

        private List<string> ScientificNotes;
        private Dictionary<char, int> octaveBonus = new Dictionary<char, int>() { { 'E',0 }, {'A', 5}, { 'D', 10}, { 'G', 15}, { 'B', 19}, { 'e', 24} };

        private string currentKey;

        private OpacityValues opacityValues = new OpacityValues();

        private string clipboard = "";
        private List<TextBlockCombination> inView = new List<TextBlockCombination>();

        private int playCount = 0;

    // General Methods
                
        public MainWindow()
        {
            InitializeComponent();
            this.ScientificNotes = new NoteDatabaseManager().getData();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    // Section C Methods

        private void MajorRadio_Checked(object sender, RoutedEventArgs e)
        {
            MinorRadio.IsChecked = false;
        }

        private void MinorRadio_Checked(object sender, RoutedEventArgs e)
        {
            MajorRadio.IsChecked = false;
        }


        private string GetSelectedKey() //Converts key held in input boxes to the Key name form used by KeyDatabase tables
        {
            string majorOrMinor;
            if (MajorRadio.IsChecked == true)
            {
                majorOrMinor = "ma";
            }
            else
            {
                majorOrMinor = "mi";
            }
            return Notes[KeySelectionBox.SelectedIndex] + majorOrMinor;
        }


        private void ResetFretboard() //Resets all buttons on Guitar Fretboard Display to red
        {
            string[] strings = {"E","A","D","G","B","e"};
            var colour = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFC90000"));
            for (int i = 0; i< strings.Length;i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    Button FBE = (Button)FindName(strings[i] + j.ToString());
                    FBE.Background = colour;
                    FBE.BorderBrush = colour;
                }
            }
        }

        private void SetGreen(string whichString, List<int> frets) //Sets the frets passed in to green on a particular guitar string
        {
            var colour = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF6BF955"));
            for (int i = 0; i < frets.Count; i++)
            {
                Button FBE = (Button)FindName(whichString + frets[i]);
                FBE.Background = colour;
                FBE.BorderBrush = colour;
            }
        }
        
        
        private void BuildNeck() //Method used to change the Guitar Fretboard Display to reflect the user's key choice
        {
            this.currentKey = GetSelectedKey();
            KeyDatabaseManager keyDatabaseManager = new KeyDatabaseManager();
            ResetFretboard();
            List<int> E = keyDatabaseManager.getData(this.currentKey, "E").FindIndexAll(x => x == 1);
            List<int> A = keyDatabaseManager.getData(this.currentKey, "A").FindIndexAll(x => x == 1);
            List<int> D = keyDatabaseManager.getData(this.currentKey, "D").FindIndexAll(x => x == 1);
            List<int> G = keyDatabaseManager.getData(this.currentKey, "G").FindIndexAll(x => x == 1);
            List<int> B = keyDatabaseManager.getData(this.currentKey, "B").FindIndexAll(x => x == 1);
            List<int> e = keyDatabaseManager.getData(this.currentKey, "p").FindIndexAll(x => x == 1);
            SetGreen("E", E);
            SetGreen("A", A);
            SetGreen("D", D);
            SetGreen("G", G);
            SetGreen("B", B);
            SetGreen("e", e);         
        }



        private void KTEnter_Click(object sender, RoutedEventArgs e) //Executes when the user clicks the enter button
        {
            KTEnterDialogue.Text = "";
            if (KeySelectionBox.SelectedIndex != -1)
            {
                if (MajorRadio.IsChecked == true || MinorRadio.IsChecked == true)
                {
                    BuildNeck();
                }
                else
                {
                    KTEnterDialogue.Text = "Please Select Either Major or Minor";
                }                                
            }
            else
            {
                KTEnterDialogue.Text = "Please Select a Key";
            }
        }

        private void Button_StringButton(object sender, RoutedEventArgs e) //Provides input for section D through clipboard field while responding to a section C element
        {
            Button button = (Button)sender;
            this.clipboard = button.Name.Remove(0, 1);
            opacityValues.ChangeOpacity(button.Name[0]);
            MakeOpaque(opacityValues.eVis, "e");
            MakeOpaque(opacityValues.BVis, "B");
            MakeOpaque(opacityValues.GVis, "G");
            MakeOpaque(opacityValues.DVis, "D");
            MakeOpaque(opacityValues.AVis, "A");
            MakeOpaque(opacityValues.EVis, "E");
        }


     // Section D Methods

        private void MakeOpaque(double opaque, string guitarString) //Makes all tab elements on a specified guitar string the passed in opacity
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    TextBlock target = (TextBlock)FindName(guitarString + "0" + (i + 1).ToString() + (j + 1).ToString());
                    target.Opacity = opaque;
                }
            }
        }


        private void Tab_MouseEnter(object sender, MouseEventArgs e) //Responds to the MouseEnter event for a tab element
        {
            if (this.clipboard != "")
            {
                TextBlock textBlock = (TextBlock)sender;
                this.inView.Add(new TextBlockCombination(textBlock, textBlock.Text));
                textBlock.Opacity = 0.5;
                textBlock.Text = this.clipboard;
                textBlock.FontSize = 32;
            }
            
        }

        private void Tab_MouseLeave(object sender, MouseEventArgs e) //Responds to the MouseLeave event for a tab element
        {
            TextBlock textBlock = (TextBlock)sender;
            TextBlockCombination result = this.inView.Find(x => x.getTextBlock() == textBlock);
            if (result != null)
            {
                textBlock.Opacity = (Double)opacityValues.GetType().GetProperty(textBlock.Name[0]+"Vis").GetValue(opacityValues, null);
                textBlock.Text = result.getPreviousValue();
                if (textBlock.Text == "X")
                {
                    textBlock.FontSize = 24;
                }
                inView.Remove(result);
            }
        }

        private void Tab_MouseLeft(object sender, MouseButtonEventArgs e) // Responds to the MouseLeftButtonDown event for a tab element
        {
            TextBlock textBlock = (TextBlock)sender;
            if (clipboard != "")
            {
                inView.Remove(inView.Find(x => x.getTextBlock() == textBlock));
                textBlock.Opacity = (Double)opacityValues.GetType().GetProperty(textBlock.Name[0] + "Vis").GetValue(opacityValues, null);
                textBlock.Text = this.clipboard;
                textBlock.FontSize = 32;
            }
            
        }

        private void Tab_MouseRight(object sender, MouseButtonEventArgs e) // Responds to the MouseRightButtonDown event for a tab element
        {
            if (clipboard != "")
            { 
                TextBlock textBlock = (TextBlock)sender;
                inView.Remove(inView.Find(x => x.getTextBlock() == textBlock));
                textBlock.Opacity = (Double)opacityValues.GetType().GetProperty(textBlock.Name[0] + "Vis").GetValue(opacityValues, null);
                textBlock.Text = "X";
                textBlock.FontSize = 24;
            }
        }

        private void RevertOpacity_Click() //Non-event handler version of RevertOpacity_Click()
        {
            opacityValues.Initialize(1.0);
            clipboard = "";
            MakeOpaque(opacityValues.eVis, "e");
            MakeOpaque(opacityValues.BVis, "B");
            MakeOpaque(opacityValues.GVis, "G");
            MakeOpaque(opacityValues.DVis, "D");
            MakeOpaque(opacityValues.AVis, "A");
            MakeOpaque(opacityValues.EVis, "E");
        }

        private void RevertOpacity_Click(object sender, RoutedEventArgs e) //Reverts the opacity of all tab elements to 1.0
        {
            opacityValues.Initialize(1.0);
            clipboard = "";
            MakeOpaque(opacityValues.eVis, "e");
            MakeOpaque(opacityValues.BVis, "B");
            MakeOpaque(opacityValues.GVis, "G");
            MakeOpaque(opacityValues.DVis, "D");
            MakeOpaque(opacityValues.AVis, "A");
            MakeOpaque(opacityValues.EVis, "E");
        }

        private void ClearNote(string guitarString) // Reverts all tab elements to default value
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    TextBlock target = (TextBlock)FindName(guitarString + "0" + (i + 1).ToString() + (j + 1).ToString());
                    target.Text = "X";
                    target.FontSize = 24;
                }
            }
        }

        private void ClearTab_Click(object sender, EventArgs e) // Responds to the Click event for the Clear Tab button
        {
            if (ClearTab_E.IsChecked == true)
            {
                ClearNote("E");
            }
            if (ClearTab_A.IsChecked == true)
            {
                ClearNote("A");
            }
            if (ClearTab_D.IsChecked == true)
            {
                ClearNote("D");
            }
            if (ClearTab_G.IsChecked == true)
            {
                ClearNote("G");
            }
            if (ClearTab_B.IsChecked == true)
            {
                ClearNote("B");
            }
            if (ClearTab_e.IsChecked == true)
            {
                ClearNote("e");
            }
            RevertOpacity_Click();
        }

        private void SaveTab_Click(object sender, EventArgs e) //Saves a screenshot of the Tab for the user
        {
            using (Bitmap bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    String filename = "Tab(" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ").png";
                    Opacity = 0;
                    graphics.CopyFromScreen((int)SystemParameters.VirtualScreenLeft, (int)SystemParameters.VirtualScreenTop, 0, 0, bitmap.Size);
                    SaveFileDialog saveFileDialogue = new SaveFileDialog();
                    saveFileDialogue.FileName = filename;
                    saveFileDialogue.DefaultExt = ".png";
                    Opacity = 1;
                    if (saveFileDialogue.ShowDialog() == true)
                    {
                        bitmap.Save(saveFileDialogue.FileName);
                    }


                }

            }
        }


        // Section E Methods

        private void SearchNote(List<NewMidiNote> newMidiNotes, string guitarString) // Searches Tab Creator guitar string row for notes adding them to newMidiNotes
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    TextBlock target = (TextBlock)FindName(guitarString + "0" + (i + 1).ToString() + (j + 1).ToString());
                    if (target.Text != "X")
                    {
                        string scientificNote = ScientificNotes[octaveBonus[target.Name[0]] + Int32.Parse(target.Text)];
                        newMidiNotes.Add(new NewMidiNote(j + (8*i), scientificNote[0].ToString()+scientificNote[1].ToString(), Int32.Parse(scientificNote[2].ToString()) ));
                    }
                }
            }
        }

        private void FillNotesList(List<NewMidiNote> newMidiNotes) // Repeatedly calls the SearchNote method for each guitar string
        {
            SearchNote(newMidiNotes, "E");
            SearchNote(newMidiNotes, "A");
            SearchNote(newMidiNotes, "D");
            SearchNote(newMidiNotes, "G");
            SearchNote(newMidiNotes, "B");
            SearchNote(newMidiNotes, "e");
        }
       

        private void PlayFretboard() //Contains the main functionality for playing the notes displayed on the Tab Creator
        {
            List<NewMidiNote> newMidiNotes = new List<NewMidiNote>();
            FillNotesList(newMidiNotes);
            List<NewMidiNote> lastMidiNotes = FindLastNotes(newMidiNotes);
            PatternBuilder patternBuilder = new PatternBuilder().SetNoteLength(MusicalTimeSpan.Eighth).Anchor(1);
            for (int i = 0; i < newMidiNotes.Count; i++)
            {
                patternBuilder.MoveToTime(new MusicalTimeSpan(newMidiNotes[i].position + 1, 8, true));
                NoteName noteName = (NoteName)NotesScientific.FindIndex(x => x == newMidiNotes[i].note);
                patternBuilder.Note(Melanchall.DryWetMidi.MusicTheory.Note.Get(noteName, newMidiNotes[i].octave));
                patternBuilder.MoveToFirstAnchor();
            }
            if (lastMidiNotes.Count != 0)
            {
                patternBuilder.SetNoteLength(MusicalTimeSpan.Whole);
                for (int i = 0; i < lastMidiNotes.Count; i++)
                {
                    patternBuilder.MoveToTime(new MusicalTimeSpan(lastMidiNotes[i].position + 1, 8, true));
                    NoteName noteName = (NoteName)NotesScientific.FindIndex(x => x == lastMidiNotes[i].note);
                    patternBuilder.Note(Melanchall.DryWetMidi.MusicTheory.Note.Get(noteName, lastMidiNotes[i].octave));
                    patternBuilder.MoveToFirstAnchor();
                }
            }


            MidiFile midiFile = patternBuilder.Build().ToFile(TempoMap.Create(new Tempo(60000000/Int32.Parse(TempoBox.Text)))); //   BPM = 60000000 / Tempo
            
            using (var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
            {
                midiFile.Play(outputDevice);
                
            }
            
        }

        private List<NewMidiNote> FindLastNotes(List<NewMidiNote> newMidiNotes) //Finds the last notes in the Tab Creator, removing them from newMidiNotes and returning them separately
        {
            List<NewMidiNote> lastMidiNotes = new List<NewMidiNote>();
            int count = 24;
            while (lastMidiNotes.Count == 0 && count != -1)
            {
                lastMidiNotes = newMidiNotes.FindAll(x => x.position == count);
                count -= 1;
            }
            if (lastMidiNotes.Count != 0)
            {
                for (int i = 0; i < lastMidiNotes.Count; i++)
                {
                    newMidiNotes.Remove(lastMidiNotes[i]);
                }
            }
            return lastMidiNotes;
        }

        private void PlayButton_MouseLeft(object sender, MouseButtonEventArgs e) // Responds to the MouseLeftButtonDown event for the play button
        {
            SettingsDialogue.Text = "";
            if (TempoBox.Text != "")
            {
                int tempoInt = 0;
                bool isInt = true;
                try
                {
                    tempoInt = Int32.Parse(TempoBox.Text);
                }
                catch (Exception)
                {
                    isInt = false;
                }
                if (tempoInt >= 30 && tempoInt <= 300 && isInt == true)
                {
                        this.playCount += 1;
                        if (this.playCount == 1)
                        {
                            PlayFretboard();
                            this.playCount = 0;
                        }
                }
                else if (isInt == false)
                {
                    SettingsDialogue.Text = "Please Input an Integer Tempo";
                }
                else
                {
                    SettingsDialogue.Text = "Tempo Needs To Be in The Range of 30 - 300";
                }
            }
            else
            {
                SettingsDialogue.Text = "Please Input Tempo";
            }


        }
        
    }
}
