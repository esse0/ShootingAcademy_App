export const TEXTS = {
    // Common
    SUCCESS: 'Success',
    ERROR: 'Error',
    CANCEL: 'Cancel',
    SAVE: 'Save',
    SAVE_CHANGES: 'Save Changes',
    CREATE: 'Create',
    DELETE: 'Delete',
    EDIT: 'Edit',
    BACK: 'Back',
    NEXT: 'Next',
    SUBMIT: 'Submit',
    LOADING: 'Loading...',
    NO_DATA: 'No data available',

    // Notifications
    COURSE_CREATED: 'Course created successfully',
    COURSE_CREATE_ERROR: 'Failed to create course',
    COURSE_UPDATED: 'Course updated successfully',
    COURSE_UPDATE_ERROR: 'Failed to update course',
    COURSE_DELETED: 'Course deleted successfully',
    COURSE_DELETE_ERROR: 'Failed to delete course',

    COMPETITION_CREATED: 'Competition created successfully',
    COMPETITION_CREATE_ERROR: 'Failed to create competition',
    COMPETITION_UPDATED: 'Competition updated successfully',
    COMPETITION_UPDATE_ERROR: 'Failed to update competition',
    COMPETITION_DELETED: 'Competition deleted successfully',
    COMPETITION_DELETE_ERROR: 'Failed to delete competition',

    // Course
    COURSE_TITLE: 'Course Title',
    COURSE_DESCRIPTION: 'Course Description',
    COURSE_DURATION: 'Course Duration',
    COURSE_LEVEL: 'Course Level',
    COURSE_CATEGORY: 'Course Category',
    COURSE_RATE: 'Course Rate',
    COURSE_MODULES: 'Course Modules',
    COURSE_LESSONS: 'Course Lessons',
    COURSE_VIDEO: 'Course Video',
    COURSE_FEATURES: 'Course Features',
    COURSE_FAQ: 'Course FAQ',
    INSTRUCTOR_DESCRIPTION: 'Instructor Description',

    // Module
    MODULE_TITLE: 'Module Title',
    MODULE_DESCRIPTION: 'Module Description',
    MODULE_ORDER: 'Module Order',
    ADD_MODULE: 'Add Module',

    // Lesson
    LESSON_TITLE: 'Lesson Title',
    LESSON_DESCRIPTION: 'Lesson Description',
    LESSON_ORDER: 'Lesson Order',
    LESSON_VIDEO: 'Lesson Video',
    ADD_LESSON: 'Add Lesson',

    // Competition
    COMPETITION_TITLE: 'Competition Title',
    COMPETITION_DESCRIPTION: 'Competition Description',
    COMPETITION_DATE: 'Competition Date',
    COMPETITION_TIME: 'Competition Time',
    COMPETITION_LOCATION: 'Competition Location',
    COMPETITION_VENUE: 'Competition Venue',
    COMPETITION_COUNTRY: 'Country',
    COMPETITION_CITY: 'City',
    COMPETITION_MAX_MEMBERS: 'Max Members Count',
    COMPETITION_EXERCISE: 'Exercise',

    // Exercises
    EXERCISES: {
        RIFLE_10M_20: 'PN rifle, 10m, 20 shots',
        RIFLE_10M_40: 'PN rifle, 10m, 40 shots',
        PISTOL_10M_60: 'Pistol, 10 meters, 60 shots',
        PISTOL_25M_60: 'Pistol, 25 meters, 60 shots'
    },

    // Validation
    REQUIRED_FIELD: 'This field is required',
    INVALID_EMAIL: 'Invalid email address',
    INVALID_NUMBER: 'Must be a positive number',
    INVALID_DATE: 'Invalid date',
    INVALID_TIME: 'Invalid time',

    // Navigation
    HOME: 'Home',
    COURSES: 'Courses',
    COMPETITIONS: 'Competitions',
    GROUPS: 'Groups',
    SETTINGS: 'Settings',
    PROFILE: 'Profile',
    LOGOUT: 'Logout',

    // Validation messages
    MODULE_TITLE_EMPTY: 'Module title is empty in module ID: ',
    LESSON_EMPTY: 'No lessons added in module ID: ',
    LESSON_TITLE_EMPTY: 'Lesson title is empty in lesson ID: ',
    LESSON_DESCRIPTION_EMPTY: 'Lesson description is empty in lesson ID: ',
    VIDEO_NOT_UPLOADED: 'Video is not uploaded in lesson ID: ',

    // Organization texts
    PARAMETERS_NOT_SPECIFIED: 'Parameters not specified',
    ORGANIZATION_ID_NOT_FOUND: 'Organization ID not found',
    CHANGES_SAVED: 'Changes saved successfully!',
    UNKNOWN_ERROR: 'Unknown error',
    ORGANIZATION_SETTINGS: 'Organization Settings',
    BASIC_INFORMATION: 'Basic Information',
    ORGANIZATION_NAME: 'Organization Name',
    ORGANIZATION_NAME_PLACEHOLDER: 'Name',
    NAME_REQUIRED: 'Name is required',
    NAME_MIN_LENGTH: 'Name must be at least 3 characters long',
    NAME_MAX_LENGTH: 'Name must not exceed 100 characters',
    DESCRIPTION: 'Description',
    ORGANIZATION_DESCRIPTION_PLACEHOLDER: 'Organization description',
    DESCRIPTION_MAX_LENGTH: 'Description must not exceed 500 characters',
    EMAIL: 'Email',
    EMAIL_REQUIRED: 'Email is required',
    INVALID_EMAIL_FORMAT: 'Invalid email format',
    PHONE: 'Phone',
    PHONE_PLACEHOLDER: '+1 XXX XXX-XXXX',
    INVALID_PHONE_FORMAT: 'Invalid phone format',
    ADDRESS: 'Address',
    ORGANIZATION_ADDRESS: 'Organization Address',
    ADDRESS_PLACEHOLDER: '123 Main St',
    ADDRESS_MAX_LENGTH: 'Address must not exceed 200 characters',
    SAVING: 'Saving...',
    ORGANIZATION_INVITATION: 'Organization Invitation',
    USER_REMOVED: 'User removed',
    ERROR_REMOVING_USER: 'Error removing user',
    FIRST_NAME: 'First Name',
    LAST_NAME: 'Last Name',
    ACTIONS: 'Actions',
    INVITE: 'Invite',

    // Error messages
    ERROR_OCCURRED: 'An error occurred',

    // User and organization texts
    USERNAME: 'Username',
    ROLE: 'Role',
    JOIN_DATE: 'Join Date',
    STATUS: 'Status',
    PENDING: 'Pending',
    APPROVED: 'Approved',
    REJECTED: 'Rejected',
    ORGANIZATION_MEMBERS: 'Organization Members',
    INVITE_USERS: 'Invite Users',
    CLOSE: 'Close',

    // Role management
    ROLE_UPDATED: 'Role updated successfully',

    // Group and coach texts
    GROUP_INVITATION: 'Group Invitation',
    COACH: 'Coach',
    COACH_NAME: 'Coach Name',
    GRADE: 'Grade',
    AGE: 'Age',

    // Location texts
    COUNTRY: 'Country',

    ORGANIZATION_RANGES: "Organization Ranges",
    ADD_RANGE: "Add Range",
    EDIT_RANGE: "Edit Range",
    RANGE_ADDED: "Range added successfully",
    RANGE_UPDATED: "Range updated successfully",
    RANGE_DELETED: "Range deleted successfully",
    ERROR_ADDING_RANGE: "Error adding range",
    ERROR_UPDATING_RANGE: "Error updating range",
    ERROR_DELETING_RANGE: "Error deleting range",
    LOCATION: "Location",
    CAPACITY: "Capacity",
    TYPE: "Type",
    ADD: "Add",
} as const; 