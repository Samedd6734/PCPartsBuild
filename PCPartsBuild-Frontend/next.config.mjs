/** @type {import('next').NextConfig} */
const nextConfig = {
  async rewrites() {
    return [
      {
        source: '/api/:path*',
        destination: 'http://localhost:5298/api/:path*', // Proxy to Backend
      },
    ];
  },
};

export default nextConfig;
