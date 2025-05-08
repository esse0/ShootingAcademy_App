import { Outlet, useLocation } from 'react-router'
import Header from '../../components/Header/Header';
import Footer from '../../components/Footer/Footer';



function AdvertisingLayout() {

  const location = useLocation();
  const hideFooter = ['/signin', '/signup'].includes(location.pathname);

  const menuItems = [
    { label: 'Home', to: '/home' },
    { label: 'Courses', to: '/courses' },
  ];

  return (
    <>
      <Header menuItems={menuItems}/>

      <Outlet/>

      {!hideFooter && <Footer menuItems={menuItems}></Footer>}
    </>
  )
}

export default AdvertisingLayout
