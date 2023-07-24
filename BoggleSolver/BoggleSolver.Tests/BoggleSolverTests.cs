using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BoggleSolver.Tests;

public class BoggleSolverTests
{
    private readonly Boggle _solver;

    public BoggleSolverTests()
    {
        _solver = new Boggle();
    }

    [Fact]
    public void SolveBoard_ReturnsCorrectWordsOnSmallBoard()
    {
        string[] legalWords = { "c" };
        _solver.SetLegalWords(legalWords);
        
        int boardWidth = 1;
        int boardHeight = 1;
        string boardLetters = "c";
        
        IEnumerable<string> result = _solver.SolveBoard(boardWidth, boardHeight, boardLetters);
        
        Assert.Contains("c", result);
        Assert.Equal(1, result.Count());
    }

    [Fact]
    public void SolveBoard_ReturnsCorrectWordsOnNonSquareBoard()
    {
        string[] legalWords = { "daze", "zeda", "daxi" };
        _solver.SetLegalWords(legalWords);
        
        int boardWidth = 3;
        int boardHeight = 2;
        string boardLetters = "dzxeai";
        
        IEnumerable<string> result = _solver.SolveBoard(boardWidth, boardHeight, boardLetters);
        
        Assert.Contains("daze", result);
        Assert.Contains("zeda", result);
        Assert.Contains("daxi", result);
        
        Assert.Equal(3, result.Count());
    }
    
    [Fact]
    public void SolveBoard_ReturnsCorrectWords()
    {
        string[] legalWords = { "dog", "hog", "dot", "octa" };
        _solver.SetLegalWords(legalWords);

        int boardWidth = 3;
        int boardHeight = 3;
        string boardLetters = "ctaodogha";

        IEnumerable<string> result = _solver.SolveBoard(boardWidth, boardHeight, boardLetters);

        Assert.Contains("dog", result);
        Assert.Contains("hog", result);
        Assert.Contains("dot", result);
        Assert.Contains("octa", result);
        Assert.Equal(4, result.Count());
    }

    [Fact]
    public void SolveBoard_ReturnsEmptyListIfNoWordsFound()
    {
        string[] legalWords = { "cat", "dog", "hat" };
        _solver.SetLegalWords(legalWords);

        int boardWidth = 3;
        int boardHeight = 3;
        string boardLetters = "xyzabcdefg";

        IEnumerable<string> result = _solver.SolveBoard(boardWidth, boardHeight, boardLetters);

        Assert.Empty(result);
    }

    [Fact]
    public void SolveBoard_ReturnsEmptyListIfLegalWordsNotSet()
    {
        int boardWidth = 3;
        int boardHeight = 3;
        string boardLetters = "abc";

        IEnumerable<string> result = _solver.SolveBoard(boardWidth, boardHeight, boardLetters);

        Assert.Empty(result);
    }
}