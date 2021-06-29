/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.Queue;
import edu.princeton.cs.algs4.StdRandom;

public class Board {
    private final int[][] board;
    private final int size;
    private final int[] zeroIndex;
    private final int manhattan;

    // create a board from an n-by-n array of tiles,
    // where tiles[row][col] = tile at (row, col)
    public Board(int[][] tiles) {
        size = tiles.length;
        board = new int[size][size];
        zeroIndex = new int[2];

        int dist = 0;
        int[] cord = new int[2];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                board[i][j] = tiles[i][j];
                if (tiles[i][j] == 0) {
                    zeroIndex[0] = i;
                    zeroIndex[1] = j;
                }
                else {
                    cord[0] = i;
                    cord[1] = j;
                    int[] properCord = where(board[i][j]);
                    dist += abs(cord[0] - properCord[0]) + abs(cord[1] - properCord[1]);
                }
            }
        }
        manhattan = dist;
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
        return manhattan;
    }

    // is this board the goal board?
    public boolean isGoal() {
        return manhattan == 0;
    }

    // does this board equal y?
    // y가 not null & 동일 값 & 동일 class -> true
    public boolean equals(Object y) {
        if (y == null || this.getClass() != y.getClass()) return false;
        if (this == y) return true;

        if (this.size != ((Board) y).size) return false;
        if (this.zeroIndex[0] != ((Board) y).zeroIndex[0]
                || this.zeroIndex[1] != ((Board) y).zeroIndex[1]) return false;
        for (int i = 0; i < this.size; i++) {
            for (int j = 0; j < this.size; j++) {
                if (this.board[i][j] != ((Board) y).board[i][j]) return false;
            }
        }
        return true;
    }

    // all neighboring boards
    public Iterable<Board> neighbors() {
        Queue<Board> queBoard = new Queue<Board>();
        if (zeroIndex[0] != 0) {
            int[] target = { zeroIndex[0] - 1, zeroIndex[1] };
            queBoard.enqueue(new Board(exchange(board, zeroIndex, target)));
        }
        if (zeroIndex[0] != size - 1) {
            int[] target = { zeroIndex[0] + 1, zeroIndex[1] };
            queBoard.enqueue(new Board(exchange(board, zeroIndex, target)));
        }
        if (zeroIndex[1] != 0) {
            int[] target = { zeroIndex[0], zeroIndex[1] - 1 };
            queBoard.enqueue(new Board(exchange(board, zeroIndex, target)));
        }
        if (zeroIndex[1] != size - 1) {
            int[] target = { zeroIndex[0], zeroIndex[1] + 1 };
            queBoard.enqueue(new Board(exchange(board, zeroIndex, target)));
        }
        return queBoard;
    }

    // a board that is obtained by exchanging any pair of tiles
    public Board twin() {
        int[] source = { StdRandom.uniform(0, size), StdRandom.uniform(0, size) };
        while (source[0] == zeroIndex[0] && source[1] == zeroIndex[1]) {
            source[0] = StdRandom.uniform(0, size);
            source[1] = StdRandom.uniform(0, size);
        }

        int[] target = { StdRandom.uniform(0, size), StdRandom.uniform(0, size) };
        while (source[0] == target[0] && source[1] == target[1]
                || target[0] == zeroIndex[0] && target[1] == zeroIndex[1]) {
            target[0] = StdRandom.uniform(0, size);
            target[1] = StdRandom.uniform(0, size);
        }
        Board randomExchBoard = new Board(exchange(board, source, target));
        return randomExchBoard;
    }

    // return {rowIndex, colIndex};
    // row: start with 0, end with size-1; col: start with 0, end with size - 1;
    private int[] where(int num) {
        int row = num / size;
        int col = num % size - 1;
        if (col == -1) {
            row -= 1;
            col = size - 1;
        }
        int[] index = { row, col };
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
        int[][] copy = new int[arg.length][arg.length];
        for (int i = 0; i < arg.length; i++) {
            for (int j = 0; j < arg[i].length; j++) {
                copy[i][j] = arg[i][j];
            }
        }
        int tmp = copy[source[0]][source[1]];
        copy[source[0]][source[1]] = copy[target[0]][target[1]];
        copy[target[0]][target[1]] = tmp;
        return copy;
    }


    // unit testing (not graded)
    public static void main(String[] args) {
        // int[][] tst = new int[][] { { 2, 3, 4 }, { 5, 6, 1 }, { 0, 8, 7 } };
        // int[][] tst1 = new int[][] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        // int[][] tst2 = new int[][] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        // int[][] tst3 = new int[][] { { 1, 2, 3 }, { 4, 5, 6 }, { 8, 0, 7 } };
        // Board bst = new Board(tst);
        // Board bst1 = new Board(tst1);
        // Board bst2 = new Board(tst2);
        // Board bst3 = new Board(tst3);
        // System.out.println(bst);
        // System.out.println(bst4);
        // System.out.println(bst3);

        // System.out.println(bst.dimension());
        // System.out.println(bst.hamming());
        // System.out.println(bst1.hamming());

        // System.out.println(bst.isGoal());
        // System.out.println(bst1.isGoal());

        // System.out.println(bst.equals(bst1));
        // System.out.println(bst2.equals(bst1));
        // System.out.println(bst1.equals(bst1));

        // System.out.println(bst1.twin());
        // System.out.println(bst1);

        int[][] tst4 = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        Board bst4 = new Board(tst4);
        System.out.println(bst4);
        System.out.println(bst4.manhattan());
        for (Board brd : bst4.neighbors()) {
            System.out.println("=========");
            System.out.println(brd);
            System.out.println(brd.manhattan());
        }
        System.out.println(bst4);
        System.out.println("***************");
        Board bst4Twin = bst4.twin();
        System.out.println(bst4Twin);
        System.out.println(bst4Twin.manhattan());
        for (Board brd : bst4Twin.neighbors()) {
            System.out.println("=========");
            System.out.println(brd);
            System.out.println(brd.manhattan());
        }

    }

}
