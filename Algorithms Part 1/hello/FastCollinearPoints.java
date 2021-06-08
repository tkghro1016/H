/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 23, 2021
 **************************************************************************** */


import edu.princeton.cs.algs4.Insertion;

public class FastCollinearPoints {
    private LineSegment[] lineList;
    private int index;

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
        index = 0;
        lineList = new LineSegment[pointList.length];
        sortByCoord(pointList);
        mainFunc(pointList);
    }

    // the number of line segments
    public int numberOfSegments() {
        return index;
    }

    // the line segments
    public LineSegment[] segments() {
        LineSegment[] copy = new LineSegment[index];
        for (int i = 0; i < index; i++) {
            copy[i] = lineList[i];
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
        if (hi <= lo + 7) {
            Insertion.sort(aux, lo, hi);
            return;
        }
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


    private Point[] resize(Point[] oldArray) {
        int n = oldArray.length;
        Point[] copy = new Point[2 * n];
        for (int i = 0; i < n; i++)
            copy[i] = oldArray[i];
        return copy;
    }

    private LineSegment[] resize(LineSegment[] oldArray) {
        int n = oldArray.length;
        LineSegment[] copy = new LineSegment[2 * n];
        for (int i = 0; i < n; i++)
            copy[i] = oldArray[i];
        return copy;
    }

    private double[] resize(double[] oldArray) {
        int n = oldArray.length;
        double[] copy = new double[2 * n];
        for (int i = 0; i < n; i++) copy[i] = oldArray[i];
        return copy;
    }

    private void mainFunc(Point[] pointList) {
        Point[] endPoint = new Point[pointList.length];
        double[] slope = new double[pointList.length];

        for (int i = 0; i < pointList.length; i++) {
            Point[] orderPoints = new Point[pointList.length - i - 1];
            for (int j = i + 1; j < pointList.length; j++) {
                if (pointList[i].compareTo(pointList[j]) == 0) {
                    throw new IllegalArgumentException();
                }
                orderPoints[j - i - 1] = pointList[j];
            }

            sortBySlope(pointList[i], orderPoints);

            for (int j = 0; j < orderPoints.length; j++) {
                int depth = 0;
                double baseSlope = pointList[i].slopeTo(orderPoints[j]);
                for (int k = j + 1; k < orderPoints.length; k++) {
                    if (pointList[i].slopeTo(orderPoints[k]) != baseSlope) {
                        if (depth > 1) {
                            if (index == slope.length) {
                                lineList = resize(lineList);
                                endPoint = resize(endPoint);
                                slope = resize(slope);
                            }
                            // 선분에 대한 정보(끝점, 기울기) 추가됨
                            if (canIn(endPoint, slope, orderPoints[k - 1], baseSlope)) {
                                lineList[index] = new LineSegment(pointList[i], orderPoints[k - 1]);
                                endPoint[index] = orderPoints[k - 1];
                                slope[index] = baseSlope;
                                index += 1;
                            }
                        }
                        j = k - 1;
                        break;
                    }
                    else if (pointList[i].slopeTo(orderPoints[k]) == baseSlope && k
                            == orderPoints.length - 1) {
                        if (depth >= 1) {
                            if (index == slope.length) {
                                lineList = resize(lineList);
                                endPoint = resize(endPoint);
                                slope = resize(slope);
                            }
                            if (canIn(endPoint, slope, orderPoints[k], baseSlope)) {
                                lineList[index] = new LineSegment(pointList[i], orderPoints[k]);
                                endPoint[index] = orderPoints[k];
                                slope[index] = baseSlope;
                                index += 1;
                            }
                        }
                        j = k - 1;
                        break;
                    }
                    else {
                        depth += 1;
                    }
                }
            }
        }
    }

    private boolean canIn(Point[] endpoints, double[] slopes, Point endPoint, double slope) {
        for (int i = 0; i < endpoints.length; i++) {
            if (slopes[i] == slope && endpoints[i] == endPoint) return false;
        }
        return true;
    }

}
