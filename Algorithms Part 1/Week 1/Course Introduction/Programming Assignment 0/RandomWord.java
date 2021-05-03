/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 16, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdOut;
import edu.princeton.cs.algs4.StdRandom;

public class RandomWord {
    public static void main(String[] args) {
        String champ = "";
        String tmp = "";
        double num = 0.0;
        for (int i = 1; !StdIn.isEmpty(); i++) {
            tmp = StdIn.readString();
            num = 1.0 / i;
            if (StdRandom.bernoulli(num)) champ = tmp;
        }

        StdOut.println(champ);
    }
}
