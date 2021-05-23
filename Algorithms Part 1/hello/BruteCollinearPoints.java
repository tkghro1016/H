/* *****************************************************************************
 *  Name:              Lee Ki Heun
 *  Coursera User ID:  tkghro1016@gmail.com
 *  Last modified:     May 23, 2021
 **************************************************************************** */

public class BruteCollinearPoints {

    private final LineSegment[] lineList;
    private final Point[] pointList;

    // finds all line segments containing 4 points
    public BruteCollinearPoints(Point[] points) {
        if (points == null) {
            throw new IllegalArgumentException();
        }
        pointList = new Point[points.length];
        for (int i = 0; i < points.length; i++) {
            if (points[i] == null) {
                throw new IllegalArgumentException();
            }
            pointList[i] = points[i];
        }
        int tail = pointList.length;
        LineSegment[] lineSegmentList = new LineSegment[4];
        int lineIndex = 0;

        for (int i = 0; i < tail - 3; i++) {
            double[] slopeList = new double[pointList.length - i - 1];
            // slope(i,j) 계산
            for (int j = i + 1; j < tail; j++) {
                if (pointList[i].equals(pointList[j])) {
                    throw new IllegalArgumentException();
                }
                slopeList[j - i - 1] = pointList[i].slopeTo(pointList[j]);
            }
            // slopeList에 3개 이상 중복된 놈들 찾기
            for (int j = 0; j < slopeList.length - 1; j++) {
                int[] index = new int[4];
                index[0] = i;
                index[1] = i + j + 1;
                int count = 2;

                for (int k = j + 1; k < slopeList.length; k++) {
                    if (slopeList[j] == slopeList[k]) {
                        index[count++] = i + k + 1;
                        if (count == 4) {
                            lineSegmentList[lineIndex++] = new LineSegment(findMin(index),
                                                                           findMax(index));
                            if (lineIndex == lineSegmentList.length)
                                lineSegmentList = resize(lineSegmentList);
                        }
                    }
                }
            }
        }
        lineList = properSize(lineSegmentList);
    }

    // the number of line segments
    public int numberOfSegments() {
        return lineList.length;
    }

    // the line segments
    public LineSegment[] segments() {
        LineSegment[] copy = new LineSegment[lineList.length];
        for (int i = 0; i < lineList.length; i++) {
            copy[i] = lineList[i];
        }
        return copy;
    }


    private LineSegment[] resize(LineSegment[] lineSegmentList) {
        int n = lineSegmentList.length;
        LineSegment[] copy = new LineSegment[2 * n];
        for (int i = 0; i < n; i++)
            copy[i] = lineSegmentList[i];
        return copy;
    }

    private LineSegment[] properSize(LineSegment[] lineSegmentList) {
        int n = 0;
        for (int i = 0; i < lineSegmentList.length; i++) {
            if (lineSegmentList[i] == null) {
                break;
            }
            n += 1;
        }
        LineSegment[] copy = new LineSegment[n];
        for (int i = 0; i < n; i++)
            copy[i] = lineSegmentList[i];
        return copy;
    }


    private Point findMax(int[] index) {
        Point maxPoint = pointList[index[0]];
        for (int i = 1; i < index.length; i++) {
            if (maxPoint.compareTo(pointList[index[i]]) < 0) {
                maxPoint = pointList[index[i]];
            }
        }
        return maxPoint;
    }

    private Point findMin(int[] index) {
        Point minPoint = pointList[index[0]];
        for (int i = 1; i < index.length; i++) {
            if (minPoint.compareTo(pointList[index[i]]) > 0) {
                minPoint = pointList[index[i]];
            }
        }
        return minPoint;
    }
}
