$(document).ready(function () {
    
    function isTokenExpired(token) {
        if (!token) return true;

        // Decode the token payload 
        const payload = JSON.parse(atob(token.split('.')[1]));
        console.log(payload);
        const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
        console.log(currentTime);
        return payload.exp < currentTime;
    }
    if (isTokenExpired(localStorage.getItem('authToken'))) {
        localStorage.removeItem('authToken');
        localStorage.removeItem('UserID');
    }
    const token = localStorage.getItem('authToken');
    if (!token) {
        window.location.href = 'index.html';
    }
    const userId = localStorage.getItem('UserID');
    // Show loading spinner
    $('#loading').show();

    // Fetch tasks from the server
    $.ajax({
        url: `/api/Task/user/${userId}`,
        method: 'GET',
        headers: {
            Authorization: `Bearer ${token}` 
        },
        success: function (data) {
            $('#loading').hide(); // Hide loading spinner after fetching data

            if (data.length === 0) {
                $('#taskTableBody').append(`
                    <tr>
                        <td colspan="6" class="text-center"><span>No Tasks found</span></td>
                    </tr>`
                );
            } else {
                const rows = data.map(task => {
                    const formattedDate = moment(task.dueDate).format('MMMM Do YYYY');
                    const statusBadge = getStatusBadge(task.status);
                    const priorityText = getPriority(task.priority);
                    return `
                       <tr data-id="${task.id}">
                           <td>${task.title}</td>
                           <td>${task.description}</td>
                           <td>${formattedDate}</td>
                           <td>${priorityText}</td>
                           <td>${statusBadge}</td>
                           <td>
                               <button class="btn btn-sm btn-primary edit-task-btn" data-toggle="tooltip" title="Edit Task"><i class="fas fa-edit"></i> Edit</button>
                               <button class="btn btn-sm btn-danger delete-task-btn" data-toggle="tooltip" title="Delete Task"><i class="fas fa-trash-alt"></i> Delete</button>
                               <a href="taskDetail.html?id=${task.id}" class="btn btn-sm btn-info" data-toggle="tooltip" title="View Task Details"><i class="fas fa-info-circle"></i> Details</a>
                           </td>
                       </tr>`;
                }).join('');               
            $('#taskTableBody').html(rows);
            }

            $('#taskTable').DataTable({
                "paging": true,
                "pageLength": 5,
                "lengthMenu": [5, 10, 20, 50],
                "ordering": false,
                "destroy": true // Allows reinitializing the DataTable after every Ajax call
            });
        }
,
        error: function (error) {
            $('#loading').hide(); // Hide loading spinner on error
            console.error('Error fetching tasks:', error);
            alert('An error occurred while fetching tasks. Please try again later.');
        }
    });

    // Function to return status badge based on task status
    function getStatusBadge(status) {
        switch (status) {
            case 0:
                return '<span class="badge badge-secondary">ToDo</span>';
            case 1:
                return '<span class="badge badge-warning">InProgress</span>';
            case 2:
                return '<span class="badge badge-success">Completed</span>';
            case 3:
                return '<span class="badge badge-danger">Blocked</span>';
            default:
                return '<span class="badge badge-light">Unknown</span>';
        }
    }

    // Function to return priority text based on priority number
    function getPriority(priority) {
        switch (priority) {
            case 0:
                return '<span class="badge badge-success">Low</span>';  
            case 1:
                return '<span class="badge badge-warning">Medium</span>'; 
            case 2:
                return '<span class="badge badge-danger">High</span>'; 
            case 3:
                return '<span class="badge badge-dark">Critical</span>';  
            default:
                return '<span class="badge badge-secondary">Unknown</span>';  
        }
    }

    function getStatusNumber(statusText) {
        switch (statusText) {
            case 'ToDo':
                return 0;
            case 'InProgress':
                return 1;
            case 'Completed':
                return 2;
            case 'Blocked':
                return 3;
            default:
                return -1; // Unknown or invalid status
        }
    }

    function getPriorityNumber(priorityText) {
        switch (priorityText) {
            case 'Low':
                return 0;
            case 'Medium':
                return 1;
            case 'High':
                return 2;
            case 'Critical':
                return 3;
            default:
                return -1; // Unknown or invalid priority
        }
    }

    // Handle form submission for adding or editing tasks
    $('#taskForm').on('submit', function (event) {
        event.preventDefault();
        const taskId = $('#taskId').val(); // Get the task ID from the hidden input
        const title = $('#taskTitle').val().trim();
        const description = $('#taskDescription').val().trim();
        const dueDate = $('#taskDueDate').val();
        const priority = $('#taskPriority').val();
        const status = $('#taskStatus').val();
        const CreatedById = localStorage.getItem("UserID");
       

        if (title === '' || dueDate === '' || priority === '' || status === '') {
            $('#error-message').text('Please fill out all required fields.').show();
            return;
        }
        const prioriity = getPriorityNumber(priority);
        const statuus = getStatusNumber(status);
        // Determine if this is an add or edit operation
        const isEdit = taskId ? true : false;
        const requestUrl = isEdit ? `/api/Task/${taskId}` : '/api/Task';
        const requestMethod = isEdit ? 'PUT' : 'POST';

        const taskData = {
            title: title,
            description: description,
            dueDate: dueDate,
            priority: prioriity,
            status: statuus,
            CreatedById: userId
        };
        if (isEdit) {
            taskData.id = taskId;
        }

        // Perform AJAX request to send data to the server
        $.ajax({
            url: requestUrl,
            method: requestMethod,
            data: JSON.stringify(taskData),
            contentType: 'application/json',
            success: function () {
                alert(taskId ? 'Task updated successfully' : 'Task added successfully');
                location.reload(); // Refresh the page to see the new or updated task
            },
            error: function (error) {
                console.error(`Error ${taskId ? 'updating' : 'adding'} task:`, error);
                $('#error-message').text(`An error occurred while ${taskId ? 'updating' : 'adding'} the task. Please try again later.`).show();
            }
        });
    });

    // Open modal for editing a task
    $(document).on('click', '.edit-task-btn', function () {
        const taskRow = $(this).closest('tr');
        const taskId = taskRow.data('id');

        // Populate the form with the existing task data
        $('#taskId').val(taskId); // Store the task ID in the hidden input
        $('#taskTitle').val(taskRow.find('td').eq(0).text());
        $('#taskDescription').val(taskRow.find('td').eq(1).text());
        $('#taskDueDate').val(moment(taskRow.find('td').eq(2).text(), 'MMMM Do YYYY').format('YYYY-MM-DD'));
        $('#taskPriority').val(taskRow.find('td').eq(3).text());
        $('#taskStatus').val(taskRow.find('td').eq(4).text());

        $('#taskForm').data('taskId', taskId); // Store taskId in the form
        $('#taskModalLabel').text('Edit Task'); // Change the modal title to 'Edit Task'
        $('#taskModal').modal('show');
    });


    // Handle task deletion
    $(document).on('click', '.delete-task-btn', function () {
        const taskRow = $(this).closest('tr');
        const taskId = taskRow.data('id');

        if (confirm('Are you sure you want to delete this task?')) {
            $.ajax({
                url: `/api/Task/${taskId}`,
                method: 'DELETE',
                success: function () {
                    alert('Task deleted successfully');
                    taskRow.remove(); // Remove the task from the table
                },
                error: function (error) {
                    console.error('Error deleting task:', error);
                    $('#error-message').text('An error occurred while deleting the task. Please try again later.').show();
                }
            });
        }
    });

    // Clear form fields and reset modal for new task
    $('#clearForm, #addNewTaskButton').on('click', function () {
        $('#taskForm').trigger('reset'); // Reset all form fields
        $('#taskId').val(''); // Clear the hidden task ID
        $('#error-message').hide(); // Hide any error messages
        $('#taskModalLabel').text('New Task'); // Change the modal title to 'New Task'
        $('#taskModal').modal('show'); // Show the modal
    });

    // Search/filter tasks
    $('#searchInput').on('keyup', function () {
        const value = $(this).val().toLowerCase();
        $('#taskTableBody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });
    // Logout function
    $('#logout-link').click(function (event) {
        event.preventDefault(); // Prevent the immediate navigation

        // Remove token from localStorage
        localStorage.removeItem('authToken');

        // Hide and show relevant links after logging out
        $('#dashboard-link, #tasks-link, #teams-link, #logout-link').hide();
        $('#login-link').show();

        // Redirect to login page after logout
        window.location.href = 'index.html';
    });
});
