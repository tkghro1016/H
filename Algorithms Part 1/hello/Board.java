/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

public class Board {
    private int[][] board;
    private final int size;

    // create a board from an n-by-n array of tiles,
    // where tiles[row][col] = tile at (row, col)
    public Board(int[][] tiles) {
        size = tiles.length;
        board = tiles.clone();
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

        return 0;
    }

    // is this board the goal board?
    public boolean isGoal() {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (i == size - 1 && j == size - 1) break;
                if (board[i][j] != i * size + j + 1) return false;
            }
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
        int[] source = new int[] { uniform(0, size), uniform(0, size) };
        int[] target = new int[] { uniform(0, size), uniform(0, size) };
        while (source[0] == target[0] && source[1] == target[1]) {
            target[0] = uniform(0, size);
            target[1] = uniform(0, size);
        }
        Board exchBoard = new Board(exchange(board, source, target));
        return exchBoard;
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
        System.out.println(bst.twin().toString());
    }

}
