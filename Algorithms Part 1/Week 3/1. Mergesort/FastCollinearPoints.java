/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 23, 2021
 **************************************************************************** */

public class FastCollinearPoints {
    private LineSegment[] lineList;
    private Point[] pointList;
    private int index;

    // finds all line segments containing 4 or more points
    public FastCollinearPoints(Point[] points) {
        if (points == null) {
            throw new IllegalArgumentException();
        }


        // input을 바로 가리키지 않게 복사
        pointList = new Point[points.length];
        for (int i = 0; i < points.length; i++) {
            if (points[i] == null) {
                throw new IllegalArgumentException();
            }
            pointList[i] = points[i];
        }
        index = 0;
        lineList = new LineSegment[pointList.length];
        sortByCoord();
        mainFunc();
        lineList = segments();
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

    private void mergeByCoord(Point[] aux, int lo, int mid, int hi) {
        for (int k = lo; k <= hi; k++) {
            aux[k] = pointList[k];
        }

        int i = lo, j = mid + 1;
        for (int k = lo; k <= hi; k++) {
            if (i > mid) pointList[k] = aux[j++];
            else if (j > hi) pointList[k] = aux[i++];
            else if (aux[i].compareTo(aux[j]) > 0) pointList[k] = aux[j++];
            else pointList[k] = aux[i++];
        }
    }

    private void sortByCoord(Point[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sortByCoord(aux, lo, mid);
        sortByCoord(aux, mid + 1, hi);
        if (pointList[mid + 1].compareTo(pointList[mid]) > 0) return;
        mergeByCoord(aux, lo, mid, hi);
    }

    private void sortByCoord() {
        Point[] aux = new Point[pointList.length];
        sortByCoord(aux, 0, pointList.length - 1);
    }

    private void mergeBySlope(Point source, Point[] orderList, Point[] aux, int lo, int mid,
                              int hi) {
        for (int k = lo; k <= hi; k++)
            aux[k] = orderList[k];

        int i = lo, j = mid + 1;
        for (int k = lo; k <= hi; k++) {
            if (i > mid) orderList[k] = aux[j++];
            else if (j > hi) orderList[k] = aux[i++];
            else if (source.slopeOrder().compare(aux[i], aux[j]) > 0) orderList[k] = aux[j++];
            else orderList[k] = aux[i++];
        }
    }

    private void sortBySlope(Point source, Point[] orderList, Point[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sortBySlope(source, orderList, aux, lo, mid);
        sortBySlope(source, orderList, aux, mid + 1, hi);
        if (source.slopeOrder().compare(orderList[mid + 1], orderList[mid]) > 0) return;
        mergeBySlope(source, orderList, aux, lo, mid, hi);
    }

    private void sortBySlope(Point source, Point[] orderList) {
        Point[] aux = new Point[orderList.length];
        sortBySlope(source, orderList, aux, 0, orderList.length - 1);
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

    private void mainFunc() {
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

            for (int j = 0; j < orderPoints.length - 2; j++) {
                double baseSlope = pointList[i].slopeTo(orderPoints[j]);
                for (int k = j + 1; k < orderPoints.length; k++) {
                    if (pointList[i].slopeTo(orderPoints[k]) != baseSlope) {
                        if (k - j > 2 && canIn(endPoint, slope, orderPoints[k - 1], baseSlope)) {
                            if (index == slope.length) {
                                lineList = resize(lineList);
                                endPoint = resize(endPoint);
                                slope = resize(slope);
                            }
                            // 선분에 대한 정보(끝점, 기울기) 추가됨
                            lineList[index] = new LineSegment(pointList[i], orderPoints[k - 1]);
                            endPoint[index] = orderPoints[k - 1];
                            slope[index] = baseSlope;
                            index += 1;
                        }
                        j = k - 1;
                        break;
                    }
                    else if (pointList[i].slopeTo(orderPoints[k]) == baseSlope && k
                            == orderPoints.length - 1) {
                        if (k - j > 1 && canIn(endPoint, slope, orderPoints[k], baseSlope)) {
                            if (index == slope.length) {
                                lineList = resize(lineList);
                                endPoint = resize(endPoint);
                                slope = resize(slope);
                            }
                            lineList[index] = new LineSegment(pointList[i], orderPoints[k]);
                            endPoint[index] = orderPoints[k];
                            slope[index] = baseSlope;
                            index += 1;
                        }
                        j = k;
                        break;
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
