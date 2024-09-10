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
    const urlParams = new URLSearchParams(window.location.search);
    const taskId = urlParams.get('id');

    $('#taskIdForAttachment').val(taskId);
    $('#taskIdForComment').val(taskId);


    //if (userRole !== 'owner' && userRole !== 'teamLeader' && userRole !== 'admin') {
    //    $('#uploadAttachmentBtn').prop('disabled', true);
    //    $('#newAttachment').prop('disabled', true);
    //}

    if (taskId) {
        $.ajax({
            url: `/api/task/${taskId}`,
            method: 'GET',
            success: function (task) {
                displayTaskDetails(task);
                fetchComments(taskId);
                fetchAttachments(taskId);
            },
            error: function (error) {
                console.error('Error fetching task details:', error);
                alert('An error occurred while fetching task details. Please try again later.');
            }
        });
    } else {
        alert('No task ID provided.');
    }

    // Function to get the priority text and color
    function getPriorityDetails(priority) {
        switch (priority) {
            case 0:
                return { text: 'Low', color: 'badge-success' };
            case 1:
                return { text: 'Medium', color: 'badge-warning' };
            case 2:
                return { text: 'High', color: 'badge-danger' };
            case 3:
                return { text: 'Critical', color: 'badge-dark' };
            default:
                return { text: 'Unknown', color: 'badge-secondary' };
        }
    }

    // Function to get the status text and color
    function getStatusDetails(status) {
        switch (status) {
            case 0:
                return { text: 'To Do', color: 'badge-secondary' };
            case 1:
                return { text: 'In Progress', color: 'badge-warning' };
            case 2:
                return { text: 'Completed', color: 'badge-success' };
            case 3:
                return { text: 'Blocked', color: 'badge-danger' };
            default:
                return { text: 'Unknown', color: 'badge-light' };
        }
    }

    // Display task details
    function displayTaskDetails(task) {
        const priorityDetails = getPriorityDetails(parseInt(task.priority));
        const statusDetails = getStatusDetails(parseInt(task.status));

        $('#taskDetails').html(`
            <h3>${task.title}</h3>
            <p><strong>Description:</strong> ${task.description}</p>
            <p><strong>Due Date:</strong> ${moment(task.dueDate).format('MMMM Do YYYY')}</p>
            <p><strong>Priority:</strong> <span class="badge ${priorityDetails.color}">${priorityDetails.text}</span></p>
            <p><strong>Status:</strong> <span class="badge ${statusDetails.color}">${statusDetails.text}</span></p>
        `);
    }

    // Fetch comments
    function fetchComments(taskId) {
        $.ajax({
            url: `/api/comment/${taskId}`,
            method: 'GET',
            success: function (comments) {
                $('#commentsSection').empty(); // Clear previous comments
                if (comments.length > 0) {
                    comments.forEach(comment => {
                        $('#commentsSection').append(`
                        <div class="comment">
                            <p><strong>${comment.user.userName}</strong> <small>(${moment(comment.createdAt).format('MMMM Do YYYY, h:mm a')})</small></p>
                            <p>${comment.content}</p>
                        </div>
                    `);
                    });
                } else {
                    $('#commentsSection').append('<p>No comments found.</p>');
                }
            },
            error: function (error) {
                console.error('Error fetching comments:', error);
            }
        });
    }

    // Fetch attachments
    function fetchAttachments(taskId) {
        $.ajax({
            url: `/api/attachments/${taskId}`,
            method: 'GET',
            success: function (attachments) {
                $('#attachmentsSection').empty(); // Clear previous attachments
                if (attachments.length > 0) {
                    attachments.forEach(attachment => {
                        $('#attachmentsSection').append(`
                        <div class="attachment">
                            <a href="${attachment.filePath}" target="_blank">${attachment.fileName}</a>
                            <small>Uploaded by: ${attachment.uploadedBy.userName} on ${moment(attachment.uploadedAt).format('MMMM Do YYYY, h:mm a')}</small>
                        </div>
                    `);
                    });
                } else {
                    $('#attachmentsSection').append('<p>No attachments found.</p>');
                }
            },
            error: function (error) {
                console.error('Error fetching attachments:', error);
            }
        });
    }

    // Submit new comment
    $('#commentForm').on('submit', function (event) {
        event.preventDefault();
        const newComment = $('#newComment').val().trim();
        const taskId = $('#taskIdForComment').val();
        if (newComment) {
            $.ajax({
                url: `/task/comments`,
                method: 'POST',
                data: JSON.stringify({ content: newComment, taskEntityId: taskId }),
                contentType: 'application/json',
                success: function (response) {
                    $('#newComment').val('');
                    fetchComments(taskId); // Refresh comments
                },
                error: function (error) {
                    console.error('Error adding comment:', error);
                }
            });
        } else {
            alert('Please enter a comment before submitting.');
        }
    });

    // Submit new attachment
    $('#attachmentForm').on('submit', function (event) {
        event.preventDefault();
        const formData = new FormData();
        const fileInput = $('#newAttachment')[0];
        const taskId = $('#taskIdForAttachment').val();
        if (fileInput.files.length > 0) {
            formData.append('file', fileInput.files[0]);
            formData.append('taskEntityId', taskId);

            $.ajax({
                url: `/task/attachments`,
                method: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function () {
                    $('#newAttachment').val('');
                    fetchAttachments(taskId); // Refresh attachments
                },
                error: function (error) {
                    console.error('Error uploading attachment:', error);
                }
            });
        } else {
            alert('Please select a file before submitting.');
        }
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
