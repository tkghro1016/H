/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 21, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.In;
import edu.princeton.cs.algs4.MinPQ;
import edu.princeton.cs.algs4.Stack;
import edu.princeton.cs.algs4.StdOut;

public class Solver {
    private final Stack<Board> gameTree;
    private int moveCount;

    // find a solution to the initial board (using the A* algorithm)
    public Solver(Board initial) {
        if (initial == null) throw new IllegalArgumentException();

        gameTree = new Stack<Board>();

        MinPQ<Node> nodeList = new MinPQ<Node>();
        MinPQ<Node> twinNodeList = new MinPQ<Node>();

        Node crtNode = new Node(null, initial, 0);
        Node crtTwinNode = new Node(null, initial.twin(), 0);

        nodeList.insert(crtNode);
        twinNodeList.insert(crtTwinNode);

        while (nodeList.min() != null) {
            for (Board neighbor : crtNode.currentBrd.neighbors()) {
                if (crtNode.prevNode == null) {
                    Node tmpNode = new Node(crtNode, neighbor, crtNode.moves + 1);
                    nodeList.insert(tmpNode);
                }
                else {
                    if (neighbor.equals(crtNode.prevNode.currentBrd)) continue;
                    Node tmpNode = new Node(crtNode, neighbor, crtNode.moves + 1);
                    nodeList.insert(tmpNode);
                }
            }

            for (Board twinNghbr : crtTwinNode.currentBrd.neighbors()) {
                if (crtTwinNode.prevNode == null) {
                    Node tmpNode = new Node(crtTwinNode, twinNghbr, crtTwinNode.moves + 1);
                    twinNodeList.insert(tmpNode);
                }
                else {
                    if (twinNghbr.equals(crtTwinNode.prevNode.currentBrd)) continue;
                    Node tmpNode = new Node(crtTwinNode, twinNghbr, crtTwinNode.moves + 1);
                    twinNodeList.insert(tmpNode);
                }
            }

            crtNode = nodeList.delMin();
            crtTwinNode = twinNodeList.delMin();

            if (crtNode.currentBrd.isGoal()) {
                moveCount = crtNode.moves;
                while (crtNode != null) {
                    gameTree.push(crtNode.currentBrd);
                    crtNode = crtNode.prevNode;
                }
                break;
            }

            if (crtTwinNode.currentBrd.isGoal()) {
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
        return gameTree;
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
            else { // this와 cmpNode가 같은priority를 가질 때
                if (this.moves > cmpNode.moves) { // this가 cmpNode보다 자식노드이면 this가 작은 것.
                    return -1;
                }
                else if (this.moves < cmpNode.moves) {
                    return 1;
                }
                else {
                    return 0;
                }
            }
        }
    }

    // test client (see below)
    public static void main(String[] args) {
        // create initial board from file
        In in = new In(args[0]);
        int n = in.readInt();
        int[][] tiles = new int[n][n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                tiles[i][j] = in.readInt();
        Board initial = new Board(tiles);

        // solve the puzzle
        Solver solver = new Solver(initial);

        // print solution to standard output
        if (!solver.isSolvable())
            StdOut.println("No solution possible");
        else {
            StdOut.println("Minimum number of moves = " + solver.moves());
            for (Board board : solver.solution())
                StdOut.println(board);
        }

        // int[][] tst = { { 2, 3, 4 }, { 5, 6, 1 }, { 0, 8, 7 } };
        // Board bst = new Board(tst);
        // Solver slv = new Solver(bst);
        // for (Board brd : slv.solution()) {
        //     System.out.println(brd);
        // }
    }
}
