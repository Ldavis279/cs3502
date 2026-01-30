#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <getopt.h>

int main(int argc, char *argv[]) {

int opt;
char *filename = NULL;
int buffer_size = 4096;


while ((opt = getopt(argc, argv, "f:b:")) != -1) {
  if (opt == 'f') {
      filename = optarg;
  }
  else if (opt == 'b') {
      buffer_size = atoi(optarg);
  }
}

FILE *input = stdin;

if (filename != NULL) {
  input = fopen(filename, "r");
  if (input == NULL) {
      printf("Error opening file\n");
      return 1;
  }
}

char buffer[buffer_size];

while (fgets(buffer, buffer_size, input) != NULL) {
  printf("%s", buffer);
}

if (filename != NULL) {
  fclose(input);
}

return 0;
}

