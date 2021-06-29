/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.MinPQ;

public class Solver {

    private boolean solvable;
    private Board preBoard;

    private MinPQ<Board> gameTree;
    private MinPQ<Board> neighbors;

    // find a solution to the initial board (using the A* algorithm)
    public Solver(Board initial) {
        if (initial == null) throw new IllegalArgumentException();

        solvable = false;
        preBoard = null;
        gameTree = new MinPQ<Board>();
        neighbors = new MinPQ<Board>();

    }

    // is the initial board solvable? (see below)
    public boolean isSolvable() {
        return solvable;
    }

    // min number of moves to solve initial board; -1 if unsolvable
    public int moves() {
        if (!solvable) {
            return -1;
        }
        else {
            return 0;
        }
    }

    // sequence of boards in a shortest solution; null if unsolvable
    public Iterable<Board> solution() {
        return gameTree;
    }

    private void reculsiveSolution(Board current, Board parent) {

    }

    // test client (see below)
    public static void main(String[] args) {
    }

}
