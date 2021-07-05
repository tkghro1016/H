import edu.princeton.cs.algs4.MinPQ;
import edu.princeton.cs.algs4.Stack;

import java.util.Iterator;

public class Solver {

    // WTH only 99.. there's a timing fix by 1.1 I need to do....

    // Opimization Tips

    // To do
    // Use a parity argument to determine whether a puzzle is unsolvable
    // (instead of two synchronous A* searches).
    // However, this will either break the API or will require a fragile dependence
    // on the toString() method, so don't do it.

    // * Exploit the fact that the difference in Manhattan distance
    //   between a board and a neighbor is either −1 or +1.

    // DONE:

    // When two search nodes have the same Manhattan priority,
    // you can break ties however you want, e.g., by comparing either the Hamming
    // or Manhattan distances of the two boards.

    /*
    Caching the Hamming and Manhattan priorities.
    To avoid recomputing the Manhattan priority of a search node
    from scratch each time during various priority queue operations,
    pre-compute its value when you construct the search node;
    save it in an instance variable;
    and return the saved value as needed.
    This caching technique is broadly applicable:
    consider using it in any situation where you are recomputing the same quantity
    many times and for which computing that quantity is a bottleneck operation.
     */

    private Stack<Board> gameTree;
    private int totalMoves;

    // find a solution to the initial board (using the A* algorithm)
    public Solver(Board initial) {
        if (initial == null) {
            throw new IllegalArgumentException("The constructor must be valid");
        }

        /////////////////////
        Board alt = initial.twin();
        MinPQ<Node> minPQalt = new MinPQ<Node>();
        /////////////////////

        MinPQ<Node> minPQ = new MinPQ<Node>();
        gameTree = new Stack<Board>();

        Node node = new Node();
        node.moves = 0;
        node.manhatten = initial.manhattan();
        node.b = initial;
        node.prev = null;

        minPQ.insert(node);

        Board ans = node.b; // not needed but handy
        /////////////////////

        Node nodealt = new Node();
        nodealt.moves = 0;
        nodealt.manhatten = alt.manhattan();
        nodealt.b = alt;
        nodealt.prev = null;

        Board ansalt = nodealt.b;
        minPQalt.insert(nodealt);

        boolean solvans = ans.isGoal();
        boolean solvalt = ansalt.isGoal();

        while (!solvans && !solvalt) {
            /////////////////////////
            Iterable<Board> iterablealt = ansalt.neighbors();
            Iterator<Board> italt = iterablealt.iterator();
            while (italt.hasNext()) {
                Node neigh = new Node();
                neigh.b = italt.next();
                if (nodealt.prev != null && neigh.b.equals(nodealt.prev.b))
                    continue;
                neigh.prev = nodealt;
                neigh.moves = neigh.prev.moves + 1;
                neigh.manhatten = neigh.b.manhattan();
                minPQalt.insert(neigh);
            }
            nodealt = minPQalt.delMin();
            ansalt = nodealt.b;
            solvalt = ansalt.isGoal();
            if (solvalt) {
                break;
            }
            //////////////////////
            Iterable<Board> iterable = ans.neighbors();
            Iterator<Board> it = iterable.iterator();
            while (it.hasNext()) {
                Node neigh = new Node();
                neigh.b = it.next();
                if (node.prev != null && neigh.b.equals(node.prev.b))
                    continue;
                neigh.prev = node;
                neigh.moves = neigh.prev.moves + 1;
                neigh.manhatten = neigh.b.manhattan();
                minPQ.insert(neigh);
            }
            if (minPQ.isEmpty()) {
                break;
            }
            node = minPQ.delMin();
            ans = node.b;
            solvans = ans.isGoal();
            /////////////////////
        }
        if (solvans) {
            // totalMoves = 0;
            totalMoves = node.moves;

            gameTree.push(node.b);

            while (node.prev != null) {
                gameTree.push(node.prev.b);
                node = node.prev;
                // totalMoves++;
            }

        }
        else {
            totalMoves = -1;
            gameTree = null;
        }

    }

    /*
        private static class BoardSol {
            private int move;
            private Board board;
            public int compareTo(BoardSol that) {
                if (this.move > that.move) {
                    return 1;
                } else if (this.move < that.move) {
                    return -1;
                } else {
                    return 0;
                }
            }
        }
        */
    private static class Node implements Comparable<Node> {
        private int moves; // The same as manhatten + moves
        private int manhatten;
        private Board b;
        private Node prev;

        public int compareTo(Node that) {
            // Because you want the lowest priority
            // The sort will automatically given you
            // from ascending order so
            // it is essentially the same as this.priority - that.priority
            if ((this.manhatten + this.moves) > (that.manhatten + that.moves)) {
                return 1;
            }
            else if ((this.manhatten + this.moves) < (that.manhatten + that.moves)) {
                return -1;
            }
            else {
                if (this.manhatten > that.manhatten) {
                    return 1;
                }
                else if (this.manhatten < that.manhatten) {
                    return -1;
                }
                else {
                    return 0;
                }
            }
        }
    }

    // is the initial board solvable? (see below)
    public boolean isSolvable() {
        return totalMoves != -1;
    }

    // min number of moves to solve initial board
    public int moves() {
        return totalMoves;
    }


    // sequence of boards in a shortest solution
    public Iterable<Board> solution() {
        return gameTree;
    }
/*
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
    }
    */

}
