/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 11, 2021
 **************************************************************************** */


import edu.princeton.cs.algs4.StdIn;
import edu.princeton.cs.algs4.StdRandom;

public class Permutation {
    public static void main(String[] args) {
        int num = Integer.parseInt(args[0]);
        int count = 0;
        // (k+1)번째 진입하려는 놈이 진입할 수 있는 확률 = 기존 놈이 남을 확률
        // the prob. that the (k+1)th element enqueue = the prob. that the enqueued elem. remain
        double aN = (double) num / (num + 1);
        if (num == 0) return;

        RandomizedQueue<String> rq = new RandomizedQueue<>();
        while (!StdIn.isEmpty()) {
            count += 1;
            if (count > num) {
                if (StdRandom.bernoulli(aN)) {
                    rq.dequeue();
                    rq.enqueue(StdIn.readString());
                }
                else {
                    StdIn.readString();
                }
                // 점화식; recurrence relation
                aN = (num * aN) / (num + aN);
            }
            else {
                rq.enqueue(StdIn.readString());
            }
        }
        for (int i = 0; i < num; i++) {
            System.out.println(rq.dequeue());
        }
    }
}
