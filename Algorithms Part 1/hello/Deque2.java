/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 09, 2021
 **************************************************************************** */

/*
import java.util.Iterator;

public class Deque2<Item> implements Iterable<Item> {

    private Item[] itemList;
    private int last;
    private int first;
    private int capacity;


    // construct an empty deque
    public Deque2() {
        itemList = (Item[]) new Object[1];
        capacity = 1;
        first = 0;
        last = 0;
    }

    // is the deque empty?
    public boolean isEmpty() {
        return itemList.length == 0;
    }

    // return the number of items on the deque
    public int size() {
        return itemList.length;
    }

    // add the item to the front
    public void addFirst(Item item) {
        if (item == null) {
            throw new IllegalArgumentException();
        }

    }

    // add the item to the back
    public void addLast(Item item) {
        if (item == null) {
            throw new IllegalArgumentException();
        }

    }

    // remove and return the item from the front
    public Item removeFirst() {
        if (isEmpty()) {
            throw new java.util.NoSuchElementException();
        }
    }

    // remove and return the item from the back
    public Item removeLast() {
        if (isEmpty()) {
            throw new java.util.NoSuchElementException();
        }
        if (last == capacity) {
            resize(2 * capacity);
        }
        itemList[currentN++] = item;
    }

    private void resize(int n) {
        capacity = 0;
        Item[] copy = (Item[]) new Object[n];
        for (int i = 0; i < n; i++) {
            copy[i] = itemList[i];
            capacity += 1;
        }
    }

    private class DequeIterator implements Iterator<Item> {
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

    // return an iterator over items in order from front to back
    public Iterator<Item> iterator() {
        return new DequeIterator();
    }

    // unit testing (required)
    public static void main(String[] args) {

    }
}
*/
