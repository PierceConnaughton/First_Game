using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Game.Systems
{
    //represents a queue of messages that can be added to
    public class MessageLog
    {
        //define max number of messages the log can store
        private static readonly int _maxLines = 9;

        //Usse a Queue to keep track of the lines of text
        //The first line added to the log will also be the first removed
        private readonly Queue<string> _lines;


        public MessageLog()
        {
            _lines = new Queue<string>();
        }

        //adds a line to the messageLog Queue
        public void Add(string message)
        {
            _lines.Enqueue(message);

            //when going past the max number of lines remove the oldest lines
            if (_lines.Count > _maxLines)
            {
                _lines.Dequeue();
            }
        }

        //Draw each line of the MessageLog queue to the MessageLog Console
        public void Draw(RLConsole console)
        {
            console.Clear();
            string[] lines = _lines.ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i + 1, lines[i], RLColor.White);
            }
        }

    }

}
