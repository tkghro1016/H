/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 09, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.StdRandom;

import java.util.Iterator;

public class RandomizedQueue<Item> implements Iterable<Item> {

    private int size = 0;
    private Node firstNode;
    private Node lastNode;
    private int randNum;
    private Node current;

    private class Node {
        private Item item;
        private Node next;
        private Node before;
    }

    // construct an empty randomized queue
    public RandomizedQueue() {
        Node node = new Node();
        firstNode = node;
        lastNode = node;
    }

    // is the randomized queue empty?
    public boolean isEmpty() {
        return size == 0;
    }

    // return the number of items on the randomized queue
    public int size() {
        return size;
    }

    // add the item
    public void enqueue(Item item) {
        if (item == null) {
            throw new IllegalArgumentException();
        }
        Node oldFirst = firstNode;
        firstNode = new Node();
        firstNode.item = item;
        firstNode.before = null;
        if (isEmpty()) {
            firstNode.next = null;
            lastNode = firstNode;
        }
        else {
            oldFirst.before = firstNode;
            firstNode.next = oldFirst;
        }
        size += 1;
    }

    // remove and return a random item
    public Item dequeue() {
        if (size == 0) {
            throw new java.util.NoSuchElementException();
        }
        Item item;
        randNum = StdRandom.uniform(0, size);
        if (randNum == 0) {
            size -= 1;
            item = firstNode.item;
            firstNode = firstNode.next;
            firstNode.before.next = null;
            firstNode.before = null;
        }
        else if (randNum == size - 1) {
            size -= 1;
            item = lastNode.item;
            lastNode = lastNode.before;
            lastNode.next.before = null;
            lastNode.next = null;
        }
        else {
            current = firstNode;
            for (int i = 0; i < randNum; i++) {
                current = current.next;
            }
            item = current.item;
            current.next.before = current.before;
            current.before.next = current.next;
        }
        return item;
    }

    // return a random item (but do not remove it)
    public Item sample() {
        if (isEmpty()) {
            throw new java.util.NoSuchElementException();
        }
        current = firstNode;
        randNum = StdRandom.uniform(0, size);
        for (int i = 0; i < randNum; i++) {
            current = current.next;
        }
        return current.item;
    }

    private class RandomizedQueueIterator implements Iterator<Item> {

        private Node currentNode = firstNode;

        public boolean hasNext() {
            return currentNode != null;
        }

        public Item next() {
            if (currentNode == null) {
                throw new java.util.NoSuchElementException();
            }
            Item item = currentNode.item;
            currentNode = currentNode.next;
            return item;
        }

        public void remove() {
            throw new UnsupportedOperationException();
        }
    }


    // return an independent iterator over items in random order
    public Iterator<Item> iterator() {
        return new RandomizedQueueIterator();
    }

    // unit testing (required)
    public static void main(String[] args) {
        RandomizedQueue<String> rq = new RandomizedQueue<>();
        System.out.println(rq.isEmpty());
        rq.enqueue("1");
        rq.enqueue("3");
        rq.enqueue("2");
        System.out.println("=================");
        for (String s : rq) {
            System.out.println(s);
        }
        System.out.println("=================");
        System.out.println(rq.sample());
        System.out.println(rq.isEmpty());
        System.out.println("=================");
        for (String s : rq) {
            System.out.println(s);
        }
        rq.dequeue();
        System.out.println("=================");
        for (String s : rq) {
            System.out.println(s);
        }
    }

}
