/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 09, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.StdRandom;

import java.util.Iterator;

public class RandomizedQueue1<Item> implements Iterable<Item> {

    private Item[] itemList;
    private int currentN = 0;
    private int capacity;
    private int randNum;

    // construct an empty randomized queue
    public RandomizedQueue1() {
        itemList = (Item[]) new Object[1];
        capacity = 1;
    }

    // is the randomized queue empty?
    public boolean isEmpty() {
        return currentN == 0;
    }

    // return the number of items on the randomized queue
    public int size() {
        return currentN;
    }

    // add the item
    public void enqueue(Item item) {
        if (item == null) {
            throw new IllegalArgumentException();
        }
        if (currentN == capacity) {
            resize(2 * capacity);
        }
        itemList[currentN++] = item;
    }

    // remove and return a random item
    public Item dequeue() {
        if (isEmpty()) {
            throw new java.util.NoSuchElementException();
        }

        randNum = StdRandom.uniform(0, currentN);
        currentN -= 1;
        if (currentN == capacity / 4 && currentN > 0) {
            resize(capacity / 2);
        }
        return itemList[randNum];
    }

    private void resize(int n) {
        capacity = 0;
        Item[] copy = (Item[]) new Object[n];
        for (int i = 0; i < n; i++) {
            copy[i] = itemList[i];
            capacity += 1;
        }
    }

    // return a random item (but do not remove it)
    public Item sample() {
        if (isEmpty()) {
            throw new java.util.NoSuchElementException();
        }
        return null;
    }

    private class RandomizedQueueIterator implements Iterator<Item> {

        private int current = 0;

        public boolean hasNext() {
            return current == capacity + 1;
        }

        public Item next() {
            if (current == currentN + 1) {
                throw new java.util.NoSuchElementException();
            }
            return itemList[++currentN];
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

    }

}
