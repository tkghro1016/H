/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.MinPQ;

import java.util.Iterator;

public class Solver {
    private MinPQ<Integer> boardPQ;

    // find a solution to the initial board (using the A* algorithm)
    public Solver(Board initial) {
        boardPQ.insert(initial.manhattan());


    }

    // is the initial board solvable? (see below)
    public boolean isSolvable() {
        return false;
    }

    // min number of moves to solve initial board; -1 if unsolvable
    public int moves() {
        return 0;
    }

    // sequence of boards in a shortest solution; null if unsolvable
    public Iterable<Board> solution() {
        return () -> new Iterator<Board>() {
            public boolean hasNext() {
                return false;
            }

            public Board next() {
                return null;
            }
        };
    }

    // test client (see below)
    public static void main(String[] args) {
    }

}
