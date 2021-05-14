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
        // queue에 모든 element를 저장한 후, 이를 뽑는 것을 생각해보면 각각의 element는 확률적으로 같다.
        // 그러므로 새로운 item이 대기열에 포함되는 확률(= 기존 item이 queue에 남아 있을 확률)은 줄을 읽을 때마다 업데이트 되어야 한다.(점화식)
        // the prob. that the (k+1)th element enqueue = the prob. that the enqueued elem. remain
        // If you think of storing all elements in the queue and then pulling one out, each element is probabilistically equal.
        // Therefore, the probability of a new item being enqueued should be updated every time a line is read. (recurrence relation)
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
                // 확률 점화식; recurrence relation of probability
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
