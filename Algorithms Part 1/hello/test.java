/* *****************************************************************************
 *  Name:              Ada Lovelace
 *  Coursera User ID:  123456
 *  Last modified:     October 16, 1842
 **************************************************************************** */

import edu.princeton.cs.algs4.MinPQ;

public class test {
    public static void main(String[] args) {
        int[][] tst4 = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
        Board bst4 = new Board(tst4);
        MinPQ brdList = new MinPQ();
        brdList.insert(bst4);
        System.out.println(brdList.delMin());

    }
}
