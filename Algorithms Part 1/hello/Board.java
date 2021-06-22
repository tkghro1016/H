/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.StdRandom;

import java.util.Iterator;

public class Board {
    private final int[][] board;
    private final int size;
    private final int[] zeroIndex;

    // create a board from an n-by-n array of tiles,
    // where tiles[row][col] = tile at (row, col)
    public Board(int[][] tiles) {
        size = tiles.length;
        board = new int[size][size];
        zeroIndex = new int[2];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                board[i][j] = tiles[i][j];
                if (tiles[i][j] == 0) {
                    zeroIndex[0] = i;
                    zeroIndex[1] = j;
                }
            }
        }
    }

    // string representation of this board
    public String toString() {
        StringBuilder buffer = new StringBuilder();
        buffer.append("" + size);
        for (int i = 0; i < size; i++) {
            buffer.append("\n");
            for (int j = 0; j < size; j++) {
                buffer.append(" " + board[i][j] + " ");
            }
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
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (board[i][j] == 0) continue;
                if (board[i][j] != i * size + j + 1) dist += 1;
            }
        }
        return dist;
    }

    // sum of Manhattan distances between tiles and goal
    public int manhattan() {
        int dist = 0;
        int[] index = new int[2];
        int[] key = new int[2];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (board[i][j] == 0) continue;
                index[0] = i;
                index[1] = j + 1;
                key = where(board[i][j]);
                dist += abs(index[0] - key[0]) + abs(index[1] - key[1]);
            }
        }
        return dist;
    }

    // is this board the goal board?
    public boolean isGoal() {
        if (board[size - 1][size - 1] != 0) return false;
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (i == size - 1 && j == size - 1) break;
                if (board[i][j] != i * size + j + 1) return false;
            }
        }
        return true;
    }

    // does this board equal y?
    // y가 not null & 동일 값 & 동일 class -> true
    public boolean equals(Object y) {
        if (y == null || this.getClass() != y.getClass()) return false;
        else if (this == y || y.toString().equals(this.toString())) return true;
        return false;
    }

    // all neighboring boards
    public Iterable<Board> neighbors() {
        return new IterableBoard();
    }

    private class IterableBoard implements Iterable<Board> {
        public Iterator<Board> iterator() {
            return new IteratorBoard();
        }
    }

    private class IteratorBoard implements Iterator<Board> {
        private int[][] currBoard = new int[size - 1][size - 1];
        private final int[][] preBoard = board;
        private final int[] zeroRowCol = zeroIndex;

        public boolean hasNext() {
            return false;
        }

        public Board next() {
            return null;
        }

        public void remove() {

        }
    }

    // a board that is obtained by exchanging any pair of tiles
    public Board twin() {
        int[] source = new int[] { StdRandom.uniform(0, size), StdRandom.uniform(0, size) };
        int[] target = new int[] { StdRandom.uniform(0, size), StdRandom.uniform(0, size) };
        while (source[0] == target[0] && source[1] == target[1]) {
            target[0] = StdRandom.uniform(0, size);
            target[1] = StdRandom.uniform(0, size);
        }
        Board randomExchBoard = new Board(exchange(board, source, target));
        return randomExchBoard;
    }

    // return {rowIndex, colIndex};
    // row: start with 0, end with size-1; col: start with 1, end with size;
    private int[] where(int num) {
        int row = num / size;
        int col = num % size;
        if (col == 0) {
            row -= 1;
            col = size;
        }
        int[] index = new int[] { row, col };
        return index;
    }

    private int abs(int num) {
        if (num >= 0) {
            return num;
        }
        else {
            return -num;
        }
    }

    private int[][] exchange(int[][] arg, int[] source, int[] target) {
        int tmp = arg[source[0]][source[1]];
        arg[source[0]][source[1]] = arg[target[0]][target[1]];
        arg[target[0]][target[1]] = tmp;
        return arg;
    }

    // unit testing (not graded)
    public static void main(String[] args) {
        int[][] tst = new int[][] { { 2, 3, 1 }, { 5, 6, 4 }, { 0, 8, 7 } };
        int[][] tst1 = new int[][] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        int[][] tst2 = new int[][] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        Board bst = new Board(tst);
        Board bst1 = new Board(tst1);
        Board bst2 = new Board(tst2);
        System.out.println(bst.toString());
        System.out.println(bst1.toString());
        System.out.println(bst.dimension());
        System.out.println(bst.hamming());
        System.out.println(bst1.hamming());
        System.out.println(bst.isGoal());
        System.out.println(bst1.isGoal());
        System.out.println(bst.equals(bst1));
        System.out.println(bst.twin().toString());
        System.out.println(bst2.equals(bst1));
        System.out.println(bst1.manhattan());
        System.out.println(bst1.twin());
    }

}


