// See htvar b = new Boggle();

using BoggleSolver;
using System.Diagnostics;

var words = new List<string>();
words.Add("abed");
words.Add("abo");
words.Add("aby");
words.Add("aero");
words.Add("aery");
words.Add("bad");
words.Add("bade");
words.Add("bead");
words.Add("bed");
words.Add("boa");
words.Add("bore");
words.Add("bored");
words.Add("box");
words.Add("boy");
words.Add("bread");
words.Add("bred");
words.Add("bro");
words.Add("broad");
words.Add("byre");
words.Add("byroad");
words.Add("dab");
words.Add("deb");
words.Add("derby");
words.Add("dev");
words.Add("oba");
words.Add("obe");
words.Add("orb");
words.Add("orbed");
words.Add("orby");
words.Add("ore");
words.Add("oread");
words.Add("read");
words.Add("reb");
words.Add("red");
words.Add("rev");
words.Add("road");
words.Add("rob");
words.Add("robe");
words.Add("robed");
words.Add("verb");
words.Add("very");
words.Add("yob");
words.Add("yore");
var b = new Boggle();
b.SetLegalWords(words);
var sw = Stopwatch.StartNew();
var result = b.SolveBoard(3, 3, "yoxrbaved");
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);
foreach (var word in result.OrderBy(x => x))
{
    Console.WriteLine(word);
}