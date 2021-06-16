/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     June 16, 2021
 **************************************************************************** */

import java.util.ArrayList;

public class FastCollinearPoints {
    private final ArrayList<LineSegment> lineList;
    private int lineIndex;

    // finds all line segments containing 4 or more points
    public FastCollinearPoints(Point[] points) {
        if (points == null) {
            throw new IllegalArgumentException();
        }

        // input을 바로 가리키지 않게 복사
        Point[] pointList = new Point[points.length];
        for (int i = 0; i < points.length; i++) {
            if (points[i] == null) {
                throw new IllegalArgumentException();
            }
            pointList[i] = points[i];
        }
        lineList = new ArrayList<>();
        lineIndex = 0;
        sortByCoord(pointList);
        mainFunc(pointList);
    }

    // the number of line segments
    public int numberOfSegments() {
        return lineList.size();
    }

    // the line segments
    public LineSegment[] segments() {
        // 직접 변수가 노출 되지 않도록 복사
        LineSegment[] copy = new LineSegment[lineIndex];
        for (int i = 0; i < lineIndex; i++) {
            copy[i] = lineList.get(i);
        }
        return copy;
    }

    private void mergeByCoord(Point[] pointList, Point[] aux, int lo, int mid, int hi) {
        int i = lo, j = mid + 1;
        for (int k = lo; k <= hi; k++) {
            if (i > mid) aux[k] = pointList[j++];
            else if (j > hi) aux[k] = pointList[i++];
            else if (pointList[i].compareTo(pointList[j]) > 0) aux[k] = pointList[j++];
            else aux[k] = pointList[i++];
        }
    }

    private void sortByCoord(Point[] pointList, Point[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sortByCoord(aux, pointList, lo, mid);
        sortByCoord(aux, pointList, mid + 1, hi);
        mergeByCoord(pointList, aux, lo, mid, hi);
    }

    private void sortByCoord(Point[] pointList) {
        Point[] aux = pointList.clone();
        sortByCoord(aux, pointList, 0, pointList.length - 1);
    }

    private void mergeBySlope(Point source, Point[] orderList, Point[] aux, int lo, int mid,
                              int hi) {
        int i = lo, j = mid + 1;
        for (int k = lo; k <= hi; k++) {
            if (i > mid) aux[k] = orderList[j++];
            else if (j > hi) aux[k] = orderList[i++];
            else if (source.slopeOrder().compare(orderList[i], orderList[j]) > 0)
                aux[k] = orderList[j++];
            else aux[k] = orderList[i++];
        }
    }

    private void sortBySlope(Point source, Point[] orderList, Point[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sortBySlope(source, aux, orderList, lo, mid);
        sortBySlope(source, aux, orderList, mid + 1, hi);
        mergeBySlope(source, orderList, aux, lo, mid, hi);
    }

    private void sortBySlope(Point source, Point[] orderList) {
        Point[] aux = orderList.clone();
        sortBySlope(source, aux, orderList, 0, orderList.length - 1);
    }

    private void mainFunc(Point[] pointList) {
        for (int i = 0; i < pointList.length; i++) {
            Point[] slopeOrderList = pointList.clone();
            sortBySlope(pointList[i], slopeOrderList);
            double skipCriterionSlope = Double.NEGATIVE_INFINITY;
            double countCriterionSlope = Double.NEGATIVE_INFINITY;
            int count = 0;
            for (int j = 1; j < slopeOrderList.length; j++) {
                double currentSlope = pointList[i].slopeTo(slopeOrderList[j]);
                if (currentSlope == Double.NEGATIVE_INFINITY)
                    throw new IllegalArgumentException();
                boolean skip = (pointList[i].compareTo(slopeOrderList[j]) > 0) || (
                        pointList[i].compareTo(slopeOrderList[j]) < 0
                                && skipCriterionSlope == currentSlope);
                if (!skip) {
                    if (count == 0) {
                        countCriterionSlope = currentSlope;
                        count += 1;
                    }
                    else {
                        if (countCriterionSlope != currentSlope && count > 2) {
                            lineList.add(lineIndex++, new LineSegment(pointList[i],
                                                                      slopeOrderList[j - 1]));
                            countCriterionSlope = currentSlope;
                            count = 1;
                        }
                        else if (countCriterionSlope == currentSlope
                                && j == slopeOrderList.length - 1 && count >= 2) {
                            lineList.add(lineIndex++, new LineSegment(pointList[i],
                                                                      slopeOrderList[j]));
                            countCriterionSlope = currentSlope;
                        }
                        else if (countCriterionSlope == currentSlope) {
                            count += 1;
                        }
                        else {
                            count = 1;
                            countCriterionSlope = currentSlope;
                        }
                    }
                }
                else {
                    if (count > 2) {
                        lineList.add(lineIndex++, new LineSegment(pointList[i],
                                                                  slopeOrderList[j - 1]));
                    }
                    count = 0;
                    skipCriterionSlope = currentSlope;
                    countCriterionSlope = Double.NEGATIVE_INFINITY;
                }
            }
        }
    }
}
