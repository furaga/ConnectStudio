import java.io.*;

class Test
{
    public static void main(String args[])
    {
        int n = 1, t = 0;
        while(n <= 100)
        {
            t = t + n;
            n = n + 1;	// •Ï‰»Ž®
        }
        System.out.println("sum of 1 to 100 is " + t);
    }
}
