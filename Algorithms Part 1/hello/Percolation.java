/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 12, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.WeightedQuickUnionUF;

public class Percolation {
    private final int n;
    private int sum = 0;
    private final boolean[] openList;
    private final boolean[] withTop;
    private final boolean[] withBottom;
    private final WeightedQuickUnionUF uf;
    private boolean percolation;

    // creates n-by-n grid, with all sites initially blocked
    public Percolation(int n) {
        if (n <= 0) {
            throw new IllegalArgumentException("index " + n + " has to larger than 0.");
        }
        this.n = n;
        uf = new WeightedQuickUnionUF(n * n);
        openList = new boolean[n * n];
        withTop = new boolean[n * n];
        withBottom = new boolean[n * n];
        percolate = false;
    }

    // opens the site (row, col) if it is not open already
    // 최상단 row, 최하단 row와의 "연결 여부"는 연결시 "root Parent로 직접 전염". -> withTop / withBottom
    // root의 top & bottom connection을 관리하고 자식 노드들은 root 의 connection을 그대로 따른다.
    // "The connection" with the top row & the bottom row is directly "transmitted to the root node" when connected.
    // Manage the root's withTop and withBottom and child nodes just follow the root's connection.
    public void open(int row, int col) {
        validate(row, col);

        int middle = twoToOne(row, col);
        if (openList[middle]) return;
        openList[middle] = true;

        sum += 1;

        int right = middle + 1;
        int left = middle - 1;
        int up = middle - n;
        int down = middle + n;
        boolean top = false;
        boolean bottom = false;

        if (row == 1) top = true;
        if (row == n) bottom = true;

        if (right < n * row) {
            if (openList[right]) {
                if (withTop[uf.find(right)] || top) {
                    top = true;
                }
                if (withBottom[uf.find(right)] || bottom) {
                    bottom = true;
                }
                uf.union(middle, right);
            }
        }

        if (left >= (row - 1) * n) {
            if (openList[left]) {
                if (withTop[uf.find(left)] || top) {
                    top = true;
                }
                if (withBottom[uf.find(left)] || bottom) {
                    bottom = true;
                }
                uf.union(left, middle);
            }
        }

        if (up >= 0) {
            if (openList[up]) {
                if (withTop[uf.find(up)] || top) {
                    top = true;
                }
                if (withBottom[uf.find(up)] || bottom) {
                    bottom = true;
                }
                uf.union(up, middle);
            }
        }

        if (down < n * n) {
            if (openList[down]) {
                if (withTop[uf.find(down)] || top) {
                    top = true;
                }
                if (withBottom[uf.find(down)] || bottom) {
                    bottom = true;
                }
                uf.union(down, middle);
            }
        }
        withTop[uf.find(middle)] = top;
        withBottom[uf.find(middle)] = bottom;
        withTop[middle] = top;
        withBottom[middle] = bottom;
        if (top && bottom) percolate = true;
    }

    // is the site (row, col) open?
    public boolean isOpen(int row, int col) {
        validate(row, col);
        return openList[twoToOne(row, col)];
    }

    // is thesite (row, col) full?
    public boolean isFull(int row, int col) {
        validate(row, col);
        return withTop[uf.find(twoToOne(row, col))];
    }

    // returns the number of open sites
    public int numberOfOpenSites() {
        return sum;
    }

    // does the system percolate?
    public boolean percolates() {
        return percolate;
    }

    private int twoToOne(int row, int col) {
        return n * (row - 1) + col - 1;
    }

    private void validate(int row, int col) {
        if ((row > n) || (row < 1) || (col > n) || (col < 1)) {
            throw new IllegalArgumentException("index is out of the range.");
        }
    }
}
