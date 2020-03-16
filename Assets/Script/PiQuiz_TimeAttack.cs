using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PiQuiz_TimeAttack : PiQuiz , IPiQuiz
{
    const int TIME = 10;

    public void Start()
    {
        Console.WriteLine("GET ITEM in score.");
    }

}