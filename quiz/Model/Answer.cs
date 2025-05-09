using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quiz.Model
{
    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        ///disi zmiany
        public bool IsUserSelected { get; set; }
       
    }
}
