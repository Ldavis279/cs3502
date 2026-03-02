#define _POSIX_C_SOURCE 200809L
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <time.h>
#include <errno.h>
#include <string.h>


// Configuration
#define NUM_ACCOUNTS 2
#define INITIAL_BALANCE 1000.0

typedef struct {
    int account_id;
    double balance;
    int transaction_count;
    pthread_mutex_t lock;
} Account;

Account accounts[NUM_ACCOUNTS];

//checks progress
volatile int progress = 0;

// Initialize accounts
void initialize_accounts() {
    for (int i = 0; i < NUM_ACCOUNTS; i++) {
        accounts[i].account_id = i;
        accounts[i].balance = INITIAL_BALANCE;
        accounts[i].transaction_count = 0;           
        pthread_mutex_init(&accounts[i].lock, NULL);
    }
}

/* STRATEGY 1: Lock Ordering ( RECOMMENDED )
*
* ALGORITHM :
* To prevent circular wait , always acquire locks in consistent order
*
* Step 1: Identify which account ID is lower
* Step 2: Lock lower ID first
* Step 3: Lock higher ID second
* Step 4: Perform transfer
* Step 5: Unlock in reverse order
*
* WHY THIS WORKS :
* - Thread 1: transfer (0 ,1) locks 0 then 1
* - Thread 2: transfer (1 ,0) locks 0 then 1 ( SAME ORDER !)
* - No circular wait possible
*
* WHICH COFFMAN CONDITION DOES THIS BREAK ?
* Answer in your report !
*/


// TODO : Implement safe_transfer_ordered (from , to , amount )
// Use the algorithm description above
// Hint : int first = ( from < to) ? from : to;
int safe_transfer_ordered(int from, int to, double amount) {


    //error handling
    if (from < 0 || from >= NUM_ACCOUNTS ||  to < 0 || to >= NUM_ACCOUNTS || from == to || amount <= 0) {
        return -1; // invalid args
    }

    int first  = (from < to) ? from : to;
    int second = (from < to) ? to   : from;

    pthread_mutex_lock(&accounts[first].lock);
    pthread_mutex_lock(&accounts[second].lock);

    if (accounts[from].balance < amount) {
        pthread_mutex_unlock(&accounts[second].lock);
        pthread_mutex_unlock(&accounts[first].lock);
        return -2; // insufficient funds
    }

    accounts[from].balance -= amount;
    accounts[to].balance += amount;

    accounts[from].transaction_count++;
    accounts[to].transaction_count++;

    progress++; //increases progress

    pthread_mutex_unlock(&accounts[second].lock);
    pthread_mutex_unlock(&accounts[first].lock);

    return 0;
}
\


//error handles, if good then applys the algorithm 
void transfer(int from_id, int to_id, double amount) {
    if (from_id < 0 || from_id >= NUM_ACCOUNTS ||  to_id < 0 || to_id >= NUM_ACCOUNTS ||   from_id == to_id || amount <= 0) {
        printf("ERROR: invalid transfer arguments\n");
        return;
    }

    safe_transfer_ordered(from_id, to_id, amount);
}





// Thread function that receives two account IDs from and to
// and performs a transfer of $10 between them.
void *transfer_thread(void *arg) {
    int *pair = (int *)arg; 

    for (int i = 0; i < 1000; i++) {
	progress++;
 transfer(pair[0], pair[1], 10.0);
    }

    return NULL;
}




int main() {
    printf("=== Phase 4: Deadlock-Free Transfer (Lock Ordering) ===\n\n");

    initialize_accounts();

    printf("Initial Balances:\n");
    for (int i = 0; i < NUM_ACCOUNTS; i++) {
        printf("Account %d: $%.2f\n", i, accounts[i].balance);
    }

    pthread_t thread1;
    pthread_t thread2;

    int pair1[2] = {0, 1};
    int pair2[2] = {1, 0};

    pthread_create(&thread1, NULL, transfer_thread, pair1);
    pthread_create(&thread2, NULL, transfer_thread, pair2);

    // Deadlock tester
    time_t start = time(NULL);
    int last_progress = progress;

    while (1) {
        sleep(1);

        if (progress != last_progress) {
            last_progress = progress;
            start = time(NULL);
        } else {
            if (time(NULL) - start >= 5) {
                printf("\nNo progress for 5 seconds -> DEADLOCK SUSPECTED.\n");
                break;
            }
        }

        // If both threads likely finished, we can stop monitoring soon.
        if (progress >= 1500) { 
            break;
        }
    }

    //should work
    pthread_join(thread1, NULL);
    pthread_join(thread2, NULL);

    printf("\nFinal Balances:\n");
    for (int i = 0; i < NUM_ACCOUNTS; i++) {
        printf("Account %d: $%.2f (%d transfers)\n",
               i, accounts[i].balance, accounts[i].transaction_count);
    }

    printf("\nProgram finished without deadlock.\n");
    return 0;
}
