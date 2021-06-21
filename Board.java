/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import static edu.princeton.cs.algs4.StdRandom.uniform;

public class Board {
    private int[] board;
    private final int size;

    // create a board from an n-by-n array of tiles,
    // where tiles[row][col] = tile at (row, col)
    public Board(int[][] tiles) {
        size = tiles.length;
        board = new int[size * size + 1];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (tiles[i][j] == 0) board[0] = i * size + j + 1;
                board[i * tiles.length + j + 1] = tiles[i][j];
            }
        }
    }

    // string representation of this board
    public String toString() {
        StringBuilder buffer = new StringBuilder();
        buffer.append("" + size + "\n");
        for (int i = 1; i < board.length; i++) {
            buffer.append(" " + board[i] + " ");
            if (i % size == 0 && i != board.length - 1) buffer.append("\n");
        }
        return buffer.toString();
    }

    // board dimension n
    public int dimension() {
        return size;
    }

    // number of tiles out of place
    public int hamming() {
        int dist = 0;
        for (int i = 1; i < board.length; i++) {
            if (i == board[0]) continue;
            if (i != board[i]) dist += 1;
        }
        return dist;
    }

    // sum of Manhattan distances between tiles and goal
    public int manhattan() {

        return 0;
    }

    // is this board the goal board?
    public boolean isGoal() {
        if (board[0] != board.length - 1) return false;
        for (int i = 1; i < board.length - 1; i++) {
            if (i != board[i]) return false;
        }
        return true;
    }

    // does this board equal y?
    public boolean equals(Object y) {
        if (this == y) return true;
        if (y == null || getClass() != y.getClass()) return false;
        return false;
    }

    // all neighboring boards
    public Iterable<Board> neighbors() {

        return null;
    }

    // a board that is obtained by exchanging any pair of tiles
    public Board twin() {
        int exch1 = uniform(1, board.length);
        int exch2 = uniform(1, board.length);
        while (exch1 != exch2) {
            exch2 = uniform(1, board.length);
        }
        Board exchBoard = new Board(exchange(this.board, exch1, exch2));
        return exchBoard;
    }

    private int[] exchange(int[] arg, int source, int target) {
        int tmp = arg[source];
        arg[source] = arg[target];
        arg[target] = tmp;
        return arg;
    }

    // unit testing (not graded)
    public static void main(String[] args) {
        int[][] tst = new int[][] { { 2, 3, 1 }, { 5, 6, 4 }, { 0, 8, 7 } };
        int[][] tst1 = new int[][] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        Board bst = new Board(tst);
        Board bst1 = new Board(tst1);
        System.out.println(bst.toString());
        System.out.println(bst1.toString());
        System.out.println(bst.dimension());
        System.out.println(bst.hamming());
        System.out.println(bst1.hamming());
        System.out.println(bst.isGoal());
        System.out.println(bst1.isGoal());
        System.out.println(bst.equals(bst1));

    }

}
