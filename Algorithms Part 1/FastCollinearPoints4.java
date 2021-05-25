/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 23, 2021
 **************************************************************************** */

public class FastCollinearPoints {
    private LineSegment[] lineList;
    private Point[] pointList;

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
        sortByCoord();
        mainFunc();
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
        mergeByCoord(aux, lo, mid, hi);
    }

    private void sortByCoord() {
        Point[] aux = new Point[pointList.length];
        sortByCoord(aux, 0, pointList.length - 1);
    }


    private void mergeBySlope(Point source, int[] orderList, int[] aux, int lo, int mid,
                              int hi) {
        for (int k = lo; k <= hi; k++) {
            aux[k] = orderList[k];
        }

        int i = lo, j = mid + 1;
        for (int k = lo; k <= hi; k++) {
            if (i > mid) orderList[k] = aux[j++];
            else if (j > hi) orderList[k] = aux[i++];
            else if (source.slopeOrder().compare(pointList[aux[i]], pointList[aux[j]]) > 0)
                orderList[k] = aux[j++];
            else orderList[k] = aux[i++];
        }
    }

    private void sortBySlope(Point source, int[] orderList, int[] aux, int lo, int hi) {
        if (hi <= lo) return;
        int mid = lo + (hi - lo) / 2;
        sortBySlope(source, orderList, aux, lo, mid);
        sortBySlope(source, orderList, aux, mid + 1, hi);
        mergeBySlope(source, orderList, aux, lo, mid, hi);
    }

    private void sortBySlope(Point source, int[] orderList) {
        int[] aux = new int[orderList.length];
        sortBySlope(source, orderList, aux, 0, orderList.length - 1);
    }

    private void makeLineSegmentList(double[][] map) {
        int n = 0;
        int lineIndex = 0;
        for (int i = 0; i < map.length; i++) {
            for (int j = 0; j < map[i].length; j++) {
                if (map[i][j] != +0.0) {
                    n += 1;
                }
                System.out.print(map[i][j] + " ");
            }
            System.out.println();
        }
        lineList = new LineSegment[n];
        for (int i = 0; i < map.length; i++) {
            for (int j = 0; j < map[i].length; j++) {
                if (map[i][j] != +0.0) {
                    lineList[lineIndex++] = new LineSegment(pointList[i], pointList[j]);
                }
            }
        }
    }

    private void mainFunc() {
        double[][] map = new double[pointList.length][pointList.length];

        for (int i = 0; i < pointList.length; i++) {
            int[] orderPoints = new int[pointList.length - i - 1];
            for (int j = i + 1; j < pointList.length; j++) {
                if (pointList[i].compareTo(pointList[j]) == 0) {
                    throw new IllegalArgumentException();
                }
                orderPoints[j - i - 1] = j;
            }

            sortBySlope(pointList[i], orderPoints);

            for (int q = 0; q < orderPoints.length; q++) {
                System.out.print(orderPoints[q]);
            }

            System.out.println("===========");

            int j = 0;
            while (j < orderPoints.length - 2) {
                double slope = pointList[i].slopeTo(pointList[orderPoints[j]]);
                if (pointList[i].slopeTo(pointList[orderPoints[j + 2]]) == slope) {
                    if (j + 2 == orderPoints.length - 1) map[i][orderPoints[j + 2]] = slope;
                    for (int k = j + 3; k < orderPoints.length; k++) {
                        if (pointList[i].slopeTo(pointList[orderPoints[k]]) != slope) {
                            map[i][orderPoints[k - 1]] = slope;
                            j = k;
                            break;
                        }
                        else if (k == orderPoints.length - 1) {
                            map[i][orderPoints[k]] = slope;
                            j = k;
                            break;
                        }
                    }
                }
                j += 1;
            }
        }
        makeLineSegmentList(map);
    }

    private void reFind() {
        reFind(0, Double.NEGATIVE_INFINITY);
    }

    private void reFind(int base, Double baseSlope) {
        Point[] orderPoints = new Point[pointList.length - i - 1];
        for (int j = base + 1; j < pointList.length; j++) {
            if (pointList[base].compareTo(pointList[j]) == 0) {
                throw new IllegalArgumentException();
            }
            orderPoints[j - base - 1] = pointList[j];

        }
    }

    public static void main(String[] args) {
        Point p0 = new Point(0, 0);
        Point p1 = new Point(-1, 1);
        Point p2 = new Point(1, 1);
        Point p3 = new Point(-2, 2);
        Point p4 = new Point(2, 2);
        Point p5 = new Point(-3, 3);
        Point p6 = new Point(3, 3);
        Point p7 = new Point(-4, 4);
        Point p8 = new Point(4, 4);
        Point p9 = new Point(5, 5);
        Point p10 = new Point(6, 5);
        Point p11 = new Point(7, 5);
        Point p12 = new Point(8, 5);
        Point[] point = { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12 };
        FastCollinearPoints fcp = new FastCollinearPoints(point);
        LineSegment[] line = fcp.segments();
        for (int i = 0; i < line.length; i++) {
            System.out.println(line[i]);
        }
    }


}
