/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.Queue;

public class Board {
    private final int[][] board;
    private final int size;
    private int rowZero;
    private int colZero;
    private final int manhattan;
    private final int hamming;

    // create a board from an n-by-n array of tiles,
    // where tiles[row][col] = tile at (row, col)
    public Board(int[][] tiles) {
        size = tiles.length;
        board = new int[size][size];

        int manDist = 0;
        int hamDist = 0;
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                board[i][j] = tiles[i][j];
                if (tiles[i][j] == 0) {
                    rowZero = i;
                    colZero = j;
                }
                else {
                    int[] properCord = where(board[i][j]);
                    manDist += abs(i - properCord[0]) + abs(j - properCord[1]);
                    if (board[i][j] != i * size + j + 1) hamDist += 1;
                }
            }
        }
        manhattan = manDist;
        hamming = hamDist;
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
        return hamming;
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
        if (y == null) return false;
        if (this == y) return true;
        if (this.getClass() != y.getClass()) return false;
        if (this.size != ((Board) y).size) return false;
        if (this.rowZero != ((Board) y).rowZero || this.colZero != ((Board) y).colZero) {
            return false;
        }
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
        if (rowZero != 0) {
            exchange(rowZero, colZero, rowZero - 1, colZero);
            queBoard.enqueue(new Board(board));
            exchange(rowZero, colZero, rowZero - 1, colZero);
        }
        if (rowZero != size - 1) {
            exchange(rowZero, colZero, rowZero + 1, colZero);
            queBoard.enqueue(new Board(board));
            exchange(rowZero, colZero, rowZero + 1, colZero);
        }
        if (colZero != 0) {
            exchange(rowZero, colZero, rowZero, colZero - 1);
            queBoard.enqueue(new Board(board));
            exchange(rowZero, colZero, rowZero, colZero - 1);
        }
        if (colZero != size - 1) {
            exchange(rowZero, colZero, rowZero, colZero + 1);
            queBoard.enqueue(new Board(board));
            exchange(rowZero, colZero, rowZero, colZero + 1);
        }
        return queBoard;
    }

    // a board that is obtained by exchanging any pair of tiles
    public Board twin() {
        int rowSource = 0;
        int colSource = 0;
        if (board[0][0] == 0) colSource += 1;

        int rowTarget = 1;
        int colTarget = 0;
        if (board[1][0] == 0) colTarget += 1;

        exchange(rowSource, colSource, rowTarget, colTarget);
        Board brd = new Board(board);
        exchange(rowSource, colSource, rowTarget, colTarget);
        return brd;
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

    private void exchange(int sourceRow, int sourceCol, int targetRow, int targetCol) {
        int tmp = board[sourceRow][sourceCol];
        board[sourceRow][sourceCol] = board[targetRow][targetCol];
        board[targetRow][targetCol] = tmp;
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
        Board bst4Twin2 = bst4.twin();
        System.out.println(bst4Twin2);
    }

}
