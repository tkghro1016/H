/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 06, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.StdRandom;
import edu.princeton.cs.algs4.StdStats;

public class PercolationStats {
    private static final double CONFIDENCE_95 = 1.96;
    private final double[] threshold;
    private final int trials;

    // perform independent trials on an n-by-n grid
    public PercolationStats(int n, int trials) {
        if (trials <= 0) {
            throw new IllegalArgumentException("index is out of the range.");
        }

        this.trials = trials;
        threshold = new double[trials];
        int randCol;
        int randRow;
        double count;
        for (int i = 0; i < trials; i++) {
            Percolation pc = new Percolation(n);
            while (!pc.percolates()) {
                randCol = StdRandom.uniform(1, n + 1);
                randRow = StdRandom.uniform(1, n + 1);
                pc.open(randRow, randCol);
            }
            count = pc.numberOfOpenSites();
            threshold[i] = count / (n * n);
        }
    }

    // sample mean of percolation threshold
    public double mean() {
        return StdStats.mean(threshold);
    }

    // sample standard deviation of percolation threshold
    public double stddev() {
        return StdStats.stddev(threshold);
    }

    // low endpoint of 95% confidence interval
    public double confidenceLo() {
        return mean() - CONFIDENCE_95 * stddev() / Math.sqrt(trials);
    }

    // high endpoint of 95% confidence interval
    public double confidenceHi() {
        return mean() + CONFIDENCE_95 * stddev() / Math.sqrt(trials);
    }

    // test client (see below)
    public static void main(String[] args) {
        int num1 = Integer.parseInt(args[0]);
        int num2 = Integer.parseInt(args[1]);

        PercolationStats pcs = new PercolationStats(num1, num2);

        System.out.println("mean\t\t\t = " + pcs.mean());
        System.out.println("stddev\t\t\t = " + pcs.stddev());
        System.out.println("95% confidence interval\t = [" + pcs.confidenceLo()
                                   + ", " + pcs.confidenceHi() + "]");
    }

}
