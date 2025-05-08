import { Navigate, Route, Routes } from 'react-router';
import './App.css';
import HomePage from './pages/HomePage/HomePage';
import SignInPage from './pages/SignInPage/SignInPage';
import SignUpPage from './pages/SignUpPage/SignUpPage';
import AdvertisingLayout from './layouts/AdvertisingLayout/AdvertisingLayout';
import AppLayout from './layouts/AppLayout.tsx/AppLayout';
import ErrorPage from './pages/ErrorPage/ErrorPage';
import CompetitionsPage from './pages/CompetitionsPage/CompetitionsPage';
import MyActivityPage from './pages/MyActivityPage/MyActivityPage';
import CoursesPage from './pages/CoursesPage/CoursesPage';
import AboutCoursePage from './pages/AboutCoursePage/AboutCoursePage';
import CompetitionPage from './pages/CompetitionPage/CompetitionPage';
import { BackEndErrorPage } from './pages/BackEndErrorPage/BackEndErrorPage';
import GroupsPage from './pages/GroupsPage/GroupsPage';
import MyGroupPage from './pages/MyGroupPage/MyGroupPage';
import CoursePage from './pages/CoursePage/CoursePage';
import SettingsPage from './pages/SettingsPage/SettingsPage';
import CreateCompetitonPage from './pages/CreateCompetitonPage/CreateCompetitionPage';
import CreateCoursePage from './pages/CreateCoursePage/CreateCoursePage';
import CompetitionModeratePage from './pages/CompetitionModeratePage/CompetitionModeratePage';
import CourseModeratePage from './pages/CourseModeratePage/CourseModeratePage';
import { AdminPage } from './pages/AdminPage/AdminPage';

function App() {
    return (
        <Routes>
            <Route path="/" element={<AdvertisingLayout />}>
                <Route index element={<Navigate to="home" replace />} />
                <Route path="home" element={<HomePage />}></Route>
                <Route path="courses" element={<CoursesPage courseLink="/course" />}></Route>
                <Route
                    path="course/:id"
                    element={
                        <AboutCoursePage
                            sx={{
                                maxWidth: 'var(--container-width)',
                                m: '40px auto 0',
                                p: '24px',
                                gap: '100px',
                            }}
                            isApp={false}
                        />
                    }
                />
                <Route path="signin" element={<SignInPage />}></Route>
                <Route path="signup" element={<SignUpPage />}></Route>
                <Route path="*" element={<ErrorPage />}></Route>
            </Route>

            <Route path="/app" element={<AppLayout />}>
                <Route path="adminPanel" element={<AdminPage></AdminPage>}></Route>
                <Route index element={<Navigate to="myactivity" replace />} />
                <Route path="myactivity" element={<MyActivityPage />}></Route>
                <Route
                    path="courses"
                    element={<CoursesPage courseLink="/app/courses" sx={{ maxWidth: '100%', p: 0 }} />}
                />
                <Route
                    path="courses/:id"
                    element={<AboutCoursePage sx={{ m: '0 auto', p: '24px', gap: '100px' }} isApp={true} />}
                />
                <Route path="course/:id" element={<CoursePage />} />
                <Route path="competitions" element={<CompetitionsPage />} />
                <Route path="competitions/:id" element={<CompetitionPage />} />
                <Route path="moderatecompetitions" element={<CompetitionModeratePage />} />
                <Route path="moderatecompetitions/create" element={<CreateCompetitonPage />} />

                <Route path="mygroup" element={<GroupsPage />} />
                <Route path="mygroup/:id" element={<MyGroupPage />} />
                <Route path="settings" element={<SettingsPage />} />
                <Route path="moderatecourses" element={<CourseModeratePage />}></Route>
                <Route path="moderatecourses/create" element={<CreateCoursePage />} />

                <Route path="*" element={<ErrorPage />}></Route>
            </Route>

            <Route path="/error" element={<BackEndErrorPage />}></Route>
        </Routes>
    );
}

export default App;
