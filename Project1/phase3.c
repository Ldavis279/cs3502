#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <time.h>
#include <errno.h>
#include <string.h>


// Configuration - experiment with different values !
# define NUM_ACCOUNTS 2
# define INITIAL_BALANCE 1000.0


// Updated Account structure with mutex ( GIVEN )
typedef struct {
int account_id ;
double balance ;
pthread_mutex_t lock ; // NEW: Mutex for this account
} Account ;


// Global shared array - THIS CAUSES RACE CONDITIONS !
Account accounts [ NUM_ACCOUNTS ];


// progress counter to detect deadlock
volatile int progress = 0;



// GIVEN : Example of mutex initialization
void initialize_accounts () {
    for (int i = 0; i < NUM_ACCOUNTS ; i ++) {
    accounts[i].account_id = i ;
    accounts[i].balance = INITIAL_BALANCE;
    // Initialize the mutex
    pthread_mutex_init(&accounts[i].lock , NULL ) ;
    }
}

/*
// GIVEN : Conceptual example showing HOW deadlock occurs
void transfer_deadlock_example (int from_id, int to_id, double amount){
// This code WILL cause deadlock !
// Lock source account
pthread_mutex_lock (&accounts[from_id].lock ) ;
printf ("Thread %ld: Locked account %d\n", pthread_self(), from_id) ;

// Simulate processing delay
usleep (100) ;

// Try to lock destination account
printf ("Thread %ld: Waiting for account %d\n", pthread_self () , to_id) ;
pthread_mutex_lock (&accounts[to_id].lock) ; // DEADLOCK HERE !

// Transfer ( never reached if deadlocked )
accounts[from_id].balance -= amount;
accounts[to_id].balance += amount;

pthread_mutex_unlock (& accounts[to_id].lock);
pthread_mutex_unlock (& accounts[from_id].lock);
}
*/



// TODO 1: Implement complete transfer function
// Use the example above as reference
// Add balance checking ( sufficient funds ?)
// Add error handling

void transfer(int from_id, int to_id, double amount){

    //error handling
if (from_id < 0 || from_id >= NUM_ACCOUNTS || to_id < 0 || to_id >= NUM_ACCOUNTS || from_id == to_id || amount <= 0) {
    printf("ERROR: invalid transfer arguments\n");
    return;
}

// This code WILL cause deadlock !
// Lock source account
pthread_mutex_lock (&accounts[from_id].lock ) ;
printf ("Thread %ld: Locked account %d\n", pthread_self(), from_id) ;

// Simulate processing delay
usleep (10000) ;

// Try to lock destination account
printf ("Thread %ld: Waiting for account %d\n", pthread_self () , to_id) ;
pthread_mutex_lock (&accounts[to_id].lock) ; // DEADLOCK HERE !

//balance check
if(accounts[from_id].balance < amount){
    printf("Account: %d has Insufficient funds",accounts[from_id].account_id);
}
else{
// Transfer ( never reached if deadlocked )
accounts[from_id].balance -= amount;
accounts[to_id].balance += amount;
}



pthread_mutex_unlock (&accounts[to_id].lock);
pthread_mutex_unlock (&accounts[from_id].lock);
}


// Thread function that receives two account IDs from and to
// and performs a transfer of $10 between them.
void *transfer_thread(void *arg) {
    int *pair = (int *)arg;      // pair[0]=from, pair[1]=to
    transfer(pair[0], pair[1], 10.0);
    return NULL;
}


int main () {

    printf("=== Phase 3: Deadlock Demonstration ===\n\n");

    initialize_accounts();




    // TODO 2: Create threads that will deadlock
    // Thread 1: transfer (0 , 1 , amount ) // Locks 0 , wants 1
    // Thread 2: transfer (1 , 0 , amount ) // Locks 1 , wants 0
    // Result : Circular wait !

    pthread_t thread1;
    pthread_t thread2;


    int pair1[2] = {0, 1};
    int pair2[2] = {1, 0};

    pthread_create(&thread1, NULL, transfer_thread, pair1);
    pthread_create(&thread2, NULL, transfer_thread, pair2);





// TODO 3: Implement deadlock detection
// Add timeout counter in main ()
// If no progress for 5 seconds , report suspected deadlock
// Reference : time ( NULL ) for simple timing

    time_t start = time(NULL);
    int last_progress = progress;

    while (1) {
        sleep(1);

        if (progress != last_progress) {
            // progress happened, reset timer
            last_progress = progress;
            start = time(NULL);
        } 

        else {
            // no progress
            if (time(NULL) - start >= 5) {
                printf("\nNo progress for 5 seconds -> DEADLOCK SUSPECTED.\n");
                exit(0); 
            }
        }

    }


    //should never hit
  pthread_join(thread1, NULL);
  pthread_join(thread2, NULL);

// TODO 4: Document the Coffman conditions
// In your report , identify WHERE each condition occurs
// Create resource allocation graph showing circular wait
//IN REPORT
    return 0;
}




















