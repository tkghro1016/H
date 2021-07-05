/* *****************************************************************************
 *  Name:              Ada Lovelace
 *  Coursera User ID:  123456
 *  Last modified:     October 16, 1842
 **************************************************************************** */


import edu.princeton.cs.algs4.Queue;
import edu.princeton.cs.algs4.StdRandom;

public class Board {

    private final int[] board;
    private final int size;
    private int source;
    private int target;
    private int zeroIndex;
    private final int manhattan;
    private final int hamming;


    // create a board from an n-by-n array of tiles,
    // where tiles[row][col] = tile at (row, col)
    public Board(int[][] tiles) {
        if (tiles == null) throw new NullPointerException();
        if (tiles[0].length == 0) throw new NullPointerException();

        size = tiles.length;
        board = new int[size * size];

        int manDist = 0;
        int hamDist = 0;
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                board[i * size + j] = tiles[i][j];
                if (tiles[i][j] == 0) {
                    zeroIndex = i * size + j;
                }
                else {
                    int[] properCord = where(tiles[i][j]);
                    manDist += abs(i - properCord[0]) + abs(j - properCord[1]);
                    if (tiles[i][j] != i * size + j + 1) hamDist += 1;
                }
            }
        }
        manhattan = manDist;
        hamming = hamDist;

        do {
            source = StdRandom.uniform(0, size * size);
            target = StdRandom.uniform(0, size * size);
        } while (source == target || source == zeroIndex || target == zeroIndex);
    }

    // string representation of this board
    public String toString() {
        StringBuilder buffer = new StringBuilder();
        buffer.append("" + size);
        for (int i = 0; i < size; i++) {
            buffer.append("\n");
            for (int j = 0; j < size; j++) {
                buffer.append(" " + board[i * size + j] + " ");
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
    public boolean equals(Object y) {
        if (y == null || this.getClass() != y.getClass()) return false;
        if (this == y) return true;

        if (this.size != ((Board) y).size) return false;
        if (this.zeroIndex != ((Board) y).zeroIndex) return false;
        for (int i = 0; i < this.size * this.size; i++) {
            if (this.board[i] != ((Board) y).board[i]) return false;
        }
        return true;
    }

    // all neighboring boards
    public Iterable<Board> neighbors() {
        Queue<Board> queBoard = new Queue<Board>();
        // up, down, left, right
        int[] cord = { zeroIndex / size, zeroIndex % size }; // row, col
        if (cord[0] != 0) {
            int up = (cord[0] - 1) * size + cord[1];
            queBoard.enqueue(new Board(exchange(board, zeroIndex, up)));
        }
        if (cord[0] != size - 1) {
            int down = (cord[0] + 1) * size + cord[1];
            queBoard.enqueue(new Board(exchange(board, zeroIndex, down)));
        }
        if (cord[1] != 0) {
            int left = cord[0] * size + cord[1] - 1;
            queBoard.enqueue(new Board(exchange(board, zeroIndex, left)));
        }
        if (cord[1] != size - 1) {
            int right = cord[0] * size + cord[1] + 1;
            queBoard.enqueue(new Board(exchange(board, zeroIndex, right)));
        }
        return queBoard;
    }

    // a board that is obtained by exchanging any pair of tiles
    public Board twin() {
        exchange(source, target);
        Board twin = new Board(board);
        exchange(source, target);
        return twin;
    }

    private void exchange(int exchSource, int exchTarget) {
        int tmp = board[exchSource];
        board[exchSource] = board[exchTarget];
        board[exchTarget] = tmp;
    }

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

    // unit testing (not graded)
    public static void main(String[] args) {
        int[][] tst = { { 1, 3, 4 }, { 5, 6, 2 }, { 0, 8, 7 } };
        Board bst = new Board(tst);
        System.out.println(bst);
        System.out.println(bst.twin());
        for (Board brd : bst.neighbors()) {
            System.out.println("========");
            System.out.println(brd);
            System.out.println("========");
        }
    }

}
