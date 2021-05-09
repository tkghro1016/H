/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 09, 2021
 **************************************************************************** */

import edu.princeton.cs.algs4.StdRandom;

import java.util.Iterator;

public class RandomizedQueue<Item> implements Iterable<Item> {

    private Item[] itemList;
    private int capacity;
    private int tail;
    private int randNum;

    // construct an empty randomized queue
    public RandomizedQueue() {
        itemList = (Item[]) new Object[1];
        capacity = 1;
        tail = 0;
    }

    // is the randomized queue empty?
    public boolean isEmpty() {
        return tail == 0;
    }

    // return the number of items on the randomized queue
    public int size() {
        return tail;
    }

    // add the item
    public void enqueue(Item item) {
        if (item == null) {
            throw new IllegalArgumentException();
        }
        if (tail == capacity) {
            capacity = 2 * capacity;
            resize(capacity);
        }
        itemList[tail++] = item;
    }

    // remove and return a random item
    public Item dequeue() {
        if (isEmpty()) {
            throw new java.util.NoSuchElementException();
        }
        randNum = StdRandom.uniform(0, tail);
        Item tmp = itemList[randNum];
        tail -= 1;
        itemList[randNum] = itemList[tail];
        itemList[tail] = null;
        if (tail == capacity / 4 && tail > 0) {
            capacity = capacity / 2;
            resize(capacity);
        }
        return tmp;
    }

    private void resize(int n) {
        Item[] copy = (Item[]) new Object[n];
        for (int i = 0; i < tail; i++)
            copy[i] = itemList[i];
        itemList = copy;
    }

    // return a random item (but do not remove it)
    public Item sample() {
        if (isEmpty()) {
            throw new java.util.NoSuchElementException();
        }
        randNum = StdRandom.uniform(0, tail);
        return itemList[randNum];
    }

    private class RandomizedQueueIterator implements Iterator<Item> {
        Item[] copy;
        private int current;
        private int hashTail;
        private int hashCount = 0;

        RandomizedQueueIterator(int tail) {
            if (tail == 0) {
                current = 0;
            }
            hashTail = tail;
            copy = (Item[]) new Object[tail];
            for (int i = 0; i < tail; i++)
                copy[i] = itemList[i];
        }

        public boolean hasNext() {
            return hashCount != tail;
        }

        public Item next() {
            if (hashCount == tail) {
                throw new java.util.NoSuchElementException();
            }
            if (hashTail == 0) {
                return copy[0];
            }
            else {
                hashCount += 1;
                current = StdRandom.uniform(0, hashTail);
                Item itmp = copy[current];
                hashTail -= 1;
                copy[current] = copy[hashTail];
                copy[hashTail] = null;
                return itmp;
            }
        }

        public void remove() {
            throw new UnsupportedOperationException();
        }

    }


    // return an independent iterator over items in random order
    public Iterator<Item> iterator() {
        return new RandomizedQueueIterator(tail);
    }

    // unit testing (required)
    public static void main(String[] args) {
        System.out.println("RandomizedQueue");
    }
}

