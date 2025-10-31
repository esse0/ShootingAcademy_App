import { NavLink as RouterLink } from 'react-router';
import { Button, Stack } from '@mui/material';
import manImage from '../../assets/images/indus.png';
import PlayCircleOutlinedIcon from '@mui/icons-material/PlayCircleOutlined';
import SchoolIcon from '@mui/icons-material/School';
import QuizIcon from '@mui/icons-material/Quiz';
import Course from '../../components/Course/Course';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';
import HourglassBottomIcon from '@mui/icons-material/HourglassBottom';

import './HomePage.css';

function HomePage() {

    return (
      <main className="main">
        <section className='welcome_section'>
          <div className="welcome_section__content">
            <h1 className="welcome_section__title">
              Become a Professional Athlete with Expert Training & Guidance
            </h1>
            <p className="welcome_section__subtitle">
              Master the Art of Shooting,
              Comprehensive Online Courses & Personal Coaching,
              Prepare for Competitions & Elevate Your Skills,
              Join a Community of Aspiring Champions,
              Achieve Your Dreams in Shooting Sports,
              Start Your Journey Today!
            </p>
            <div className="welcome_section__buttons">
              <Button className="welcome_section__button" sx={{ bgcolor: '#455CC7' }} variant='contained' component={RouterLink} to="/signup" >Join for free</Button>
              <Button className="welcome_section__button" sx={{ color: '#455CC7' }} variant='text' component={RouterLink} to="/courses" >
                <PlayCircleOutlinedIcon className="welcome_section__button__icon" />
                Learn now
              </Button>
            </div>
          </div>
          <figure className="welcome_section__image">
            <img className="welcome_section__image__img" src={manImage} />
          </figure>
        </section>
        <section className="disciplines_section">
          <div className="disciplines_header section__header">
            <h1 className="section__title">
              Discover Popular Courses
            </h1>
            <p className="section__subtitle">
              Discover our expertly crafted shooting courses designed to enhance your skills and boost your confidence. 
              Whether you're a beginner aiming to learn the basics or an experienced shooter looking to sharpen your precision, 
              we have the perfect program for you. Join us and unlock your true potential in the art of marksmanship.
            </p>
          </div>
          <Stack gap={'60px'}> {/* Сделать подгрузку существующих курсов */}
            <Course courseLink='#' course={{id: '1', icon: undefined, level: 'Beginner', title: 'Pistol Base', rate: 4.2, duration: '2 months', description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Quisquam, quas.', category: 'Pistol'}}></Course>
            <Course courseLink='#' course={{id: '2', icon: undefined, level: 'Beginner', title: 'Indoor Shooting', rate: 4.2, duration: '2 months', description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Quisquam, quas.', category: 'Pistol'}}></Course>
            <Course courseLink='#' course={{id: '3', icon: undefined, level: 'Beginner', title: 'Indoor Shooting', rate: 4.2, duration: '2 months', description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Quisquam, quas.', category: 'Pistol'}}></Course>
            <Course courseLink='#' course={{id: '4', icon: undefined, level: 'Beginner', title: 'Indoor Shooting', rate: 4.2, duration: '2 months', description: 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Quisquam, quas.', category: 'Pistol'}}></Course>
          </Stack>
        </section>
        <section className="advantages_section">
          <div className="advantages_header section__header">
            <h1 className="section__title">
              Why learn with us?
            </h1>
            <p className="section__subtitle">
              Discover our expertly crafted shooting courses designed to enhance your skills and boost your confidence. 
              Whether you're a beginner aiming to learn the basics or an experienced shooter looking to sharpen your precision, 
              we have the perfect program for you. Join us and unlock your true potential in the art of marksmanship.
            </p>
          </div>
          <ul className="advantages__list">
            <li className="advantages__item">
              <div className="advantages__item__number">
                1 -
              </div>
              <div className="advantages__item__content">
                <div className="advantages__item__title">
                  <HourglassBottomIcon className='advantages__item__title__icon' sx={{ color: '#455CC7' }}></HourglassBottomIcon>
                  Train at Your Own Pace
                </div>
                <div className="advantages__item__subtitle">
                  Take full control of your training schedule. 
                  Access shooting lessons whenever it suits you 
                  and progress at a pace that matches your goals and lifestyle.
                </div>
              </div>
            </li>
            <li className="advantages__item flex_end">
              <div className="advantages__item__number">
                2 -
              </div>
              <div className="advantages__item__content">
                <div className="advantages__item__title">
                  <SchoolIcon className='advantages__item__title__icon' sx={{ color: '#455CC7' }}></SchoolIcon>
                  Learn from Expert Coaches
                </div>
                <div className="advantages__item__subtitle">
                  Train under the guidance of professional instructors with 
                  years of experience in competitive shooting and tactical skills. 
                  Gain practical insights and techniques to elevate your 
                  performance.
                </div>
              </div>
            </li>
            <li className="advantages__item flex_end">
              <div className="advantages__item__number">
                3 -
              </div>
              <div className="advantages__item__content">
                <div className="advantages__item__title">
                  <QuizIcon className='advantages__item__title__icon' sx={{ color: '#455CC7' }}></QuizIcon>
                  Engaging Drills and Challenges
                </div>
                <div className="advantages__item__subtitle">
                  Test your skills with interactive drills and real-world shooting scenarios. 
                  Receive instant feedback to fine-tune your abilities and ensure steady progress.
                </div>
              </div>
            </li>
            <li className="advantages__item">
              <div className="advantages__item__number">
                4 -
              </div>
              <div className="advantages__item__content">
                <div className="advantages__item__title">
                  <EmojiEventsIcon className='advantages__item__title__icon' sx={{ color: '#455CC7' }}></EmojiEventsIcon>
                  Earn Recognized Certificates
                </div>
                <div className="advantages__item__subtitle">
                  Showcase your achievements with official certificates that highlight your shooting expertise. 
                  Add them to your portfolio or share them with your network to stand out.
                </div>
              </div>
            </li>
          </ul>
        </section>
        <section className='links_section'>
          <div className="start_link">
            Ready to unlock your full potential?
            <Button className='welcome_section__button' sx={{ bgcolor: '#455CC7' }} variant='contained' component={RouterLink} to="/signup" >Start Training Today</Button>
          </div>
        </section>
      </main>
    )
  }
  
  export default HomePage;