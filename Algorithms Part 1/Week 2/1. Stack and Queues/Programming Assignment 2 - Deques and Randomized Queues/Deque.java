/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 09, 2021
 **************************************************************************** */

import java.util.Iterator;

public class Deque<Item> implements Iterable<Item> {

    private int size = 0;
    private Node firstNode;
    private Node lastNode;

    private class Node {
        private Item item;
        private Node next;
        private Node before;
    }

    // construct an empty deque
    public Deque() {
        firstNode = null;
        lastNode = null;
    }

    // is the deque empty?
    public boolean isEmpty() {
        return size == 0;
    }

    // return the number of items on the deque
    public int size() {
        return size;
    }

    // add the item to the front
    public void addFirst(Item item) {
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

    // add the item to the back
    public void addLast(Item item) {
        if (item == null) {
            throw new IllegalArgumentException();
        }
        Node oldLast = lastNode;
        lastNode = new Node();
        lastNode.item = item;
        lastNode.next = null;
        if (isEmpty()) {
            lastNode.before = null;
            firstNode = lastNode;
        }
        else {
            oldLast.next = lastNode;
            lastNode.before = oldLast;
        }
        size += 1;
    }

    // remove and return the item from the front
    public Item removeFirst() {
        if (size == 0) {
            throw new java.util.NoSuchElementException();
        }
        size -= 1;
        Item item = firstNode.item;
        if (!isEmpty()) {
            firstNode = firstNode.next;
            firstNode.before.before = null;
            firstNode.before.next = null;
            firstNode.before = null;
        }
        else {
            firstNode = null;
            lastNode = null;
        }
        return item;
    }

    // remove and return the item from the back
    public Item removeLast() {
        if (size == 0) {
            throw new java.util.NoSuchElementException();
        }
        size -= 1;
        Item item = lastNode.item;
        if (!isEmpty()) {
            lastNode = lastNode.before;
            lastNode.next.next = null;
            lastNode.next.before = null;
            lastNode.next = null;
        }
        else {
            lastNode = null;
            firstNode = null;
        }
        return item;
    }

    private class DequeIterator implements Iterator<Item> {

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

    // return an iterator over items in order from front to back
    public Iterator<Item> iterator() {
        return new DequeIterator();
    }

    // unit testing (required)
    public static void main(String[] args) {
        System.out.println("Deque");
    }
}
