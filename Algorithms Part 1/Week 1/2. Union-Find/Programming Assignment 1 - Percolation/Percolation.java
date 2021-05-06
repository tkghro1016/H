/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 06, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.WeightedQuickUnionUF;

public class Percolation {
    private final boolean[] openList;
    private final int n;
    private int sum = 0;
    private final WeightedQuickUnionUF uf;
    private final WeightedQuickUnionUF ufFull;

    // creates n-by-n grid, with all sites initially blocked
    public Percolation(int n) {
        if (n <= 0) {
            throw new IllegalArgumentException("index " + n + " is larger than 0.");
        }
        this.n = n;
        this.uf = new WeightedQuickUnionUF(n * n + 2);
        this.ufFull = new WeightedQuickUnionUF(n * n + 1);
        this.openList = new boolean[n * n + 2];
        openList[0] = true;
        openList[n * n + 1] = true;
    }

    // opens the site (row, col) if it is not open already
    public void open(int row, int col) {
        validate(row, col);

        int middle = twoToOne(row, col);
        if (openList[middle]) return;

        sum += 1;
        int left = middle - 1;
        int right = middle + 1;
        int up = middle - n;
        int down = middle + n;

        openList[middle] = true;
        if (left > (row - 1) * n) {
            if (openList[left]) {
                uf.union(left, middle);
                ufFull.union(left, middle);
            }
        }
        if (right <= row * n) {
            if (openList[right]) {
                uf.union(right, middle);
                ufFull.union(right, middle);
            }
        }
        if (up > 0) {
            if (openList[up]) {
                uf.union(up, middle);
                ufFull.union(up, middle);
            }
        }
        else {
            uf.union(0, middle);
            ufFull.union(0, middle);
        }
        if (down < n * n + 1) {
            if (openList[down]) {
                uf.union(down, middle);
                ufFull.union(down, middle);
            }
        }
        else {
            uf.union(n * n + 1, middle);
        }
    }

    // is the site (row, col) open?
    public boolean isOpen(int row, int col) {
        validate(row, col);
        return openList[twoToOne(row, col)];
    }

    // is the site (row, col) full?
    public boolean isFull(int row, int col) {
        validate(row, col);
        return (ufFull.find(twoToOne(row, col)) == ufFull.find(0));
    }

    // returns the number of open sites
    public int numberOfOpenSites() {
        return sum;
    }

    // does the system percolate?
    public boolean percolates() {
        return (uf.find(0) == uf.find(n * n + 1));
    }

    private int twoToOne(int row, int col) {
        return n * (row - 1) + col;
    }

    private void validate(int row, int col) {
        if ((row > n) || (row < 1) || (col > n) || (col < 1)) {
            throw new IllegalArgumentException("index is out of the range.");
        }
    }
}
