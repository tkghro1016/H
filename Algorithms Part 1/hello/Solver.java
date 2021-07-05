/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.MinPQ;
import edu.princeton.cs.algs4.StdOut;

import java.util.ArrayList;
import java.util.Comparator;

public class Solver {
    private ArrayList<Board> gameTree;
    private int moveCount;
    private boolean solvable;

    // find a solution to the initial board (using the A* algorithm)
    public Solver(Board initial) {
        if (initial == null) throw new NullPointerException();

        gameTree = new ArrayList<Board>();
        moveCount = 0;
        solvable = true;

        gameTree.add(initial);

        if (initial.isGoal()) {
            solvable = true;
        }
        else {
            solve(initial);
        }

    }

    // is the initial board solvable? (see below)
    public boolean isSolvable() {
        return solvable;
    }

    // min number of moves to solve initial board; -1 if unsolvable
    public int moves() {
        if (solvable) {
            return moveCount;
        }
        else {
            return -1;
        }
    }

    // sequence of boards in a shortest solution; null if unsolvable
    public Iterable<Board> solution() {
        return gameTree;
    }

    private class ComparatorBoard implements Comparator<Board> {
        public int compare(Board o1, Board o2) {
            return Integer.compare(o1.manhattan(), o2.manhattan());
        }
    }

    private void solve(Board root) {
        ComparatorBoard cmpBoard = new ComparatorBoard();
        Board preBoard = null;
        Board current = root;
        while (solvable && !current.isGoal()) {
            MinPQ<Board> neighbors = new MinPQ<Board>(cmpBoard);
            for (Board neighbor : current.neighbors()) {
                if (neighbor.equals(preBoard)) continue;
                neighbors.insert(neighbor);
            }
            preBoard = current;
            current = neighbors.delMin();
            if (preBoard.manhattan() < current.manhattan()) {
                solvable = false;
                break;
            }
            gameTree.add(current);
            moveCount += 1;
        }
    }

    // test client (see below)
    public static void main(String[] args) {
        // create initial board from file
        In in = new In(args[0]);
        int n = in.readInt();
        int[][] tiles = new int[n][n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                tiles[i][j] = in.readInt();
        Board initial = new Board(tiles);

        // solve the puzzle
        Solver solver = new Solver(initial);

        // print solution to standard output
        if (!solver.isSolvable())
            StdOut.println("No solution possible");
        else {
            StdOut.println("Minimum number of moves = " + solver.moves());
            for (Board board : solver.solution())
                StdOut.println(board);
        }
    }
}
