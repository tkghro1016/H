/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.MinPQ;
import edu.princeton.cs.algs4.Stack;

public class Solver {
    private final Stack<Board> gameTree;
    private int moveCount;

    // find a solution to the initial board (using the A* algorithm)
    public Solver(Board initial) {
        if (initial == null) throw new IllegalArgumentException();

        gameTree = new Stack<Board>();

        MinPQ<Node> nodeList = new MinPQ<Node>();
        Node crtNode = new Node(null, initial, 0);
        nodeList.insert(crtNode);

        while (nodeList.min() != null) {
            for (Board neighbor : crtNode.currentBrd.neighbors()) {
                if (crtNode.prevNode != null && neighbor.equals(crtNode.prevNode.currentBrd)) {
                    continue;
                }
                Node tmpNode = new Node(crtNode, neighbor, crtNode.moves + 1);
                nodeList.insert(tmpNode);
            }

            crtNode = nodeList.delMin();

            if (crtNode.currentBrd.isGoal()) {
                moveCount = crtNode.moves;
                while (crtNode != null) {
                    gameTree.push(crtNode.currentBrd);
                    crtNode = crtNode.prevNode;
                }
                break;
            }

            if (crtNode.currentBrd.twin().isGoal()) {
                moveCount = -1;
                break;
            }
        }
    }

    // is the initial board solvable? (see below)
    public boolean isSolvable() {
        return moveCount != -1;
    }

    // min number of moves to solve initial board; -1 if unsolvable
    public int moves() {
        return moveCount;
    }

    // sequence of boards in a shortest solution; null if unsolvable
    public Iterable<Board> solution() {
        if (isSolvable()) {
            return gameTree;
        }
        else {
            return null;
        }
    }

    private class Node implements Comparable<Node> {
        public Board currentBrd;
        public Node prevNode;
        public int manhattan;
        public int moves;
        public int priority;

        public Node(Node prevNode, Board currentBrd, int currentMove) {
            this.prevNode = prevNode;
            this.currentBrd = currentBrd;
            manhattan = currentBrd.manhattan();
            moves = currentMove;
            priority = manhattan + moves;
        }

        public int compareTo(Node cmpNode) {
            if (this.priority > cmpNode.priority) {
                return 1;
            }
            else if (this.priority < cmpNode.priority) {
                return -1;
            }
            else {
                // this와 cmpNode가 같은 priority를 가질 때
                // this가 cmpNode보다 자식노드이면 this가 작은 것.
                return Integer.compare(cmpNode.moves, this.moves);
            }
        }
    }

    // test client (see below)
    public static void main(String[] args) {
        System.out.println("Solver");
    }
}
