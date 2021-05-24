/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 23, 2021
 **************************************************************************** */

public class FastCollinearPoints {
    private LineSegment[] lineList;

    // finds all line segments containing 4 or more points
    public FastCollinearPoints(Point[] points) {
        if (points == null) {
            throw new IllegalArgumentException();
        }
        lineList = new LineSegment[points.length];

        // input을 바로 가리키지 않게 복사
        Point[] pointList = new Point[points.length];
        for (int i = 0; i < points.length; i++) {
            if (points[i] == null) {
                throw new IllegalArgumentException();
            }
            pointList[i] = points[i];
        }

        sortByCoord(pointList);

        int lineIndex = 0;

        for (int i = 0; i < pointList.length; i++) {
            Point[] orderPoints = new Point[pointList.length - i - 1];
            for (int j = i + 1; j < pointList.length; j++) {
                if (pointList[i].compareTo(pointList[j]) == 0) {
                    throw new IllegalArgumentException();
                }
                orderPoints[j - i - 1] = pointList[j];
            }

            sortBySlope(pointList[i], orderPoints);

            /**/
            for (int q = 0; q < orderPoints.length; q++) {
                System.out.println(orderPoints[q]);
            }
            for (int q = 0; q < orderPoints.length; q++) {
                System.out.print("slope: " + pointList[i].slopeTo(orderPoints[q]) + " /// ");
            }
            System.out.println();
            System.out.println("================");
            /**/

            for (int j = 0; j < orderPoints.length - 2; j++) {
                int index = 0;
                if (pointList[i].slopeOrder().compare(orderPoints[j], orderPoints[j + 2]) == 0) {
                    for (int k = j + 3; k < orderPoints.length; k++) {
                        if (pointList[i].slopeOrder().compare(orderPoints[j], orderPoints[k])
                                != 0) {
                            lineList[lineIndex++] = new LineSegment(pointList[i], pointList[k - 1]);
                            break;
                        }
                        else {
                            index += 1;
                        }
                    }
                    j = j + index;
                }
            }
        }
        properSize();
    }


    private LineSegment[] resize(LineSegment[] lineSegmentList) {
        int n = lineSegmentList.length;
        LineSegment[] copy = new LineSegment[2 * n];
        for (int i = 0; i < n; i++)
            copy[i] = lineSegmentList[i];
        return copy;
    }

    private void properSize() {
        int n = 0;
        for (int i = 0; i < lineList.length; i++) {
            if (lineList[i] == null) {
                break;
            }
            n += 1;
        }
        LineSegment[] copy = new LineSegment[n];
        for (int i = 0; i < n; i++)
            copy[i] = lineList[i];
        lineList = copy;
    }

    // the number of line segments
    public int numberOfSegments() {
        return lineList.length;
    }

    // the line segments
    public LineSegment[] segments() {
        // 직접 변수가 노출 되지 않도록 복사
        LineSegment[] copy = new LineSegment[lineList.length];
        for (int i = 0; i < lineList.length; i++) {
            copy[i] = lineList[i];
        }
        return copy;
    }


    private void mergeByCoord(Point[] pointList, Point[] aux, int lo, int mid, int hi) {
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

    private void sortByCoord(Point[] pointList, Point[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sortByCoord(pointList, aux, lo, mid);
        sortByCoord(pointList, aux, mid + 1, hi);
        mergeByCoord(pointList, aux, lo, mid, hi);
    }

    private void sortByCoord(Point[] pointList) {
        Point[] aux = new Point[pointList.length];
        sortByCoord(pointList, aux, 0, pointList.length - 1);
    }


    private void mergeBySlope(Point source, Point[] orderList, Point[] aux, int lo, int mid,
                              int hi) {
        for (int k = lo; k <= hi; k++) {
            aux[k] = orderList[k];
        }

        int i = lo, j = mid + 1;
        for (int k = lo; k <= hi; k++) {
            if (i > mid) orderList[k] = aux[j++];
            else if (j > hi) orderList[k] = aux[i++];
            else if (source.slopeOrder().compare(aux[i], aux[j]) > 0)
                orderList[k] = aux[j++];
            else orderList[k] = aux[i++];
        }
    }

    private void sortBySlope(Point source, Point[] orderList, Point[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sortBySlope(source, orderList, aux, lo, mid);
        sortBySlope(source, orderList, aux, mid + 1, hi);
        mergeBySlope(source, orderList, aux, lo, mid, hi);
    }

    private void sortBySlope(Point source, Point[] orderList) {
        Point[] aux = new Point[orderList.length];
        sortBySlope(source, orderList, aux, 0, orderList.length - 1);
    }


    public static void main(String[] args) {
        Point p1 = new Point(10000, 0);
        Point p2 = new Point(0, 10000);
        Point p3 = new Point(3000, 7000);
        Point p4 = new Point(7000, 3000);
        Point p5 = new Point(20000, 21000);
        Point p6 = new Point(3000, 4000);
        Point p7 = new Point(14000, 15000);
        Point p8 = new Point(6000, 7000);
        Point[] points = { p1, p2, p3, p4, p5, p6, p7, p8 };
        FastCollinearPoints fcp = new FastCollinearPoints(points);
        LineSegment[] segList = fcp.segments();
        System.out.println("====Segment====");
        for (int i = 0; i < segList.length; i++) {
            System.out.println(segList[i].toString());
        }
    }
}
