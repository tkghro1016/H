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

        // input을 직접 가리키지 않게 복사
        Point[] pointList = new Point[points.length];
        for (int i = 0; i < points.length; i++) {
            if (points[i] == null) {
                throw new IllegalArgumentException();
            }
            pointList[i] = points[i];
        }

        int lineIndex = 0;

        for (int i = 0; i < pointList.length; i++) {
            Point[] orderPoints = new Point[pointList.length - i - 1];
            for (int j = i + 1; j < pointList.length; j++) {
                if (pointList[i].compareTo(pointList[j]) == 0) {
                    throw new IllegalArgumentException();
                }
                orderPoints[j - i - 1] = pointList[j];
            }




            /**/
            for (int q = 0; q < orderPoints.length; q++) {
                System.out.println(orderPoints[q]);
            }
            for (int q = 0; q < orderPoints.length; q++) {
                System.out.print("slope: " + pointList[i].slopeTo(orderPoints[q]) + " /// ");
            }
            System.out.println();
            System.out.println("vvvvvvvvvvvvvvvv");

            sort(pointList[i], orderPoints);

            for (int q = 0; q < orderPoints.length; q++) {
                System.out.println(orderPoints[q]);
            }
            for (int q = 0; q < orderPoints.length; q++) {
                System.out.print("slope: " + pointList[i].slopeTo(orderPoints[q]) + " /// ");
            }
            System.out.println();
            System.out.println("================");
            /**/
            

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
        copy = null;
    }

    // the number of line segments
    public int numberOfSegments() {
        return lineList.length;
    }

    // the line segments
    public LineSegment[] segments() {
        // 직접 변수를 조정할 수 없도록 복사
        LineSegment[] copy = new LineSegment[lineList.length];
        for (int i = 0; i < lineList.length; i++) {
            copy[i] = lineList[i];
        }
        return copy;
    }

    private void merge(Point source, Point[] orderList, Point[] aux, int lo, int mid, int hi) {
        // copy to aux[]
        for (int k = lo; k <= hi; k++) {
            aux[k] = orderList[k];
        }

        // merge back to a[]
        int i = lo, j = mid + 1;
        for (int k = lo; k <= hi; k++) {
            if (i > mid) orderList[k] = aux[j++];
            else if (j > hi) orderList[k] = aux[i++];
            else if (source.slopeOrder().compare(orderList[i], orderList[j]) > 0)
                orderList[k] = aux[j++];
            else orderList[k] = aux[i++];
        }
    }

    private void sort(Point source, Point[] orderList, Point[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sort(source, orderList, aux, lo, mid);
        sort(source, orderList, aux, mid + 1, hi);
        merge(source, orderList, aux, lo, mid, hi);
    }

    private void sort(Point source, Point[] orderList) {
        Point[] aux = new Point[orderList.length];
        sort(source, orderList, aux, 0, orderList.length - 1);
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
