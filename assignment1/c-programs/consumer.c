#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <getopt.h>
#include <signal.h>

volatile sig_atomic_t shutdownFlag = 0;
volatile sig_atomic_t printStatsFlag = 0;

void handle_signal(int sig) {
    if (sig == SIGINT) {
        shutdownFlag = 1;
    }
    else if (sig == SIGUSR1) {
        printStatsFlag = 1;
    }
}

int main(int argc, char *argv[]) {

    int opt;
    int maxLines = -1;
    int verbose = 0;

    while ((opt = getopt(argc, argv, "n:v")) != -1) {
        if (opt == 'n') {
            maxLines = atoi(optarg);
        }
        else if (opt == 'v') {
            verbose = 1;
        }
    }

    signal(SIGINT, handle_signal);
    signal(SIGUSR1, handle_signal);

    char buffer[4096];
    int lines = 0;
    int characters = 0;

    while (!shutdownFlag && fgets(buffer, sizeof(buffer), stdin) != NULL) {

        if (maxLines != -1 && lines >= maxLines)
            break;

        lines++;

        for (int i = 0; buffer[i] != '\0'; i++)
            characters++;

        if (verbose)
            printf("%s", buffer);

        if (printStatsFlag) {
            fprintf(stderr, "Lines: %d\n", lines);
            fprintf(stderr, "Characters: %d\n", characters);
            printStatsFlag = 0;
        }
    }

    fprintf(stderr, "Final Lines: %d\n", lines);
    fprintf(stderr, "Final Characters: %d\n", characters);

    return 0;
}





